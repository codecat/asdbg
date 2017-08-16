#pragma once

#include <angelscript.h>

namespace dbg
{
	void Initialize(asIScriptContext* ctx);
	void Release();

	void Break();
}

#ifdef ASDBG_IMPL

#include <vector>
#include <thread>
#include <atomic>
#include <mutex>
#include <condition_variable>

#include <cstdint>

#define EZS_CPP
#include "ezsock.h"

namespace dbg
{
	static asIScriptContext* _ctx = nullptr;

	static std::mutex _logMutex;
	static std::mutex _clientsMutex;

	static std::atomic<bool> _initialized = false;
	static std::thread _networkThread;
	static std::vector<std::thread> _clientThreads;
	static std::vector<EzSock*> _clientSockets;

	static std::atomic<bool> _dbgStateBroken = false;

	static std::mutex _dbgStateNotifyStepMutex;
	static std::unique_lock<std::mutex> _dbgStateNotifyStepLock(_dbgStateNotifyStepMutex);
	static std::condition_variable _dbgStateNotifyStep;

	static void Log(const char* format, ...)
	{
		_logMutex.lock();

		printf("[ASDBG] ");

		va_list vl;
		va_start(vl, format);
		vprintf(format, vl);
		va_end(vl);

		printf("\n");

		_logMutex.unlock();
	}

	static void Send_Location(EzSock* sock)
	{
		const char* sectionName = nullptr;
		int col = 0;
		int line = _ctx->GetLineNumber(0, &col, &sectionName);

		uint16_t packetType = 1;
		sock->SendRaw(&packetType, sizeof(uint16_t));

		uint16_t lenFilename = (uint16_t)strlen(sectionName);
		sock->SendRaw(&lenFilename, sizeof(uint16_t));
		sock->SendRaw(sectionName, lenFilename);
		sock->SendRaw(&line, sizeof(int));
		sock->SendRaw(&col, sizeof(int));
	}

	static void ScriptLineCallback(asIScriptContext* ctx)
	{
		if (!_dbgStateBroken) {
			return;
		}

		_clientsMutex.lock();
		for (EzSock* sock : _clientSockets) {
			Send_Location(sock);
		}
		_clientsMutex.unlock();

		_dbgStateNotifyStep.wait(_dbgStateNotifyStepLock);
	}

	static void ClientPacket_Step(EzSock* sock)
	{
		_dbgStateNotifyStep.notify_one();
	}

	static void ClientPacket_Pause(EzSock* sock)
	{
		_dbgStateBroken = true;
	}

	static void ClientPacket_Resume(EzSock* sock)
	{
		bool wasBroken = _dbgStateBroken;
		_dbgStateBroken = false;
		if (wasBroken) {
			_dbgStateNotifyStep.notify_one();
		}
	}

	static void ClientThreadFunction(EzSock* sock)
	{
		Log("Connection accepted from: %s", inet_ntoa(sock->addr.sin_addr));

		if (_dbgStateBroken) {
			Send_Location(sock);
		}

		while (_initialized) {
			uint16_t packetType;
			if (sock->Receive(&packetType, sizeof(uint32_t)) == -1) {
				break;
			}

			bool invalidPacket = false;
			switch (packetType) {
			case 1: ClientPacket_Step(sock); break;
			case 2: ClientPacket_Pause(sock); break;
			case 3: ClientPacket_Resume(sock); break;
			default: invalidPacket = true; break;
			}

			if (invalidPacket) {
				Log("Invalid packet type received from client");
				break;
			}
		}

		sock->close();
		//delete sock; //TODO
		Log("Connection closed");
	}

	static void NetworkThreadFunction()
	{
		const int sockPort = 8912;

		EzSock sock;
		sock.create();
		sock.bind(sockPort);
		if (!sock.listen()) {
			Log("Failed to listen on port %d", sockPort);
			return;
		}
		Log("Socket listening on port %d", sockPort);

		while (_initialized) {
			EzSock* client = new EzSock;
			if (!sock.accept(client)) {
				Log("Failed to accept client!");
				continue;
			}

			_clientsMutex.lock();

			_clientSockets.push_back(client);
			_clientThreads.push_back(std::thread(ClientThreadFunction, client));

			_clientsMutex.unlock();
		}

		sock.close();
		Log("Socket closed");
	}

	void Initialize(asIScriptContext* ctx)
	{
		_ctx = ctx;
		_ctx->SetLineCallback(asFUNCTION(dbg::ScriptLineCallback), nullptr, asCALL_CDECL);

		_initialized = true;
		_networkThread = std::thread(NetworkThreadFunction);
	}

	void Release()
	{
		_initialized = false;

		_clientsMutex.lock();
		for (std::thread &clientThread : _clientThreads) {
			clientThread.join();
		}
		_clientsMutex.unlock();
		_networkThread.join();
	}

	void Break()
	{
		_dbgStateBroken = true;
	}
}

#endif

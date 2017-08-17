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
#include <string>

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

	static std::string StrFormat(const char* format, ...)
	{
		const int minBufferSize = 100;
		char* buffer = (char*)malloc(minBufferSize);

		va_list vl;
		va_start(vl, format);
		int len = vsnprintf(buffer, minBufferSize, format, vl);
		va_end(vl);

		if (len >= minBufferSize) {
			buffer = (char*)realloc(buffer, len + 1);

			va_list vl;
			va_start(vl, format);
			vsnprintf(buffer, len + 1, format, vl);
			va_end(vl);
		}

		std::string ret = buffer;
		free(buffer);
		return ret;
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

	static void Send_LocalClear(EzSock* sock)
	{
		uint16_t packetType = 2;
		sock->SendRaw(&packetType, sizeof(uint16_t));
	}

	static void Send_Local(EzSock* sock, const char* name, int typeID, void* ptr)
	{
		uint16_t packetType = 3;
		sock->SendRaw(&packetType, sizeof(uint16_t));

		uint16_t lenName = (uint16_t)strlen(name);
		sock->SendRaw(&lenName, sizeof(uint16_t));
		sock->SendRaw(name, lenName);

		std::string typeName;
		std::string value;

		asITypeInfo* typeInfo = _ctx->GetEngine()->GetTypeInfoById(typeID);
		if (typeInfo != nullptr) {
			typeName = typeInfo->GetName();
		}

		switch (typeID) {
		case asTYPEID_BOOL:
			typeName = "bool";
			if (*(bool*)ptr) {
				value = "true";
			} else {
				value = "false";
			}
			break;

		case asTYPEID_INT8: typeName = "int8"; value = std::to_string(*(int8_t*)ptr); break;
		case asTYPEID_INT16: typeName = "int16"; value = std::to_string(*(int16_t*)ptr); break;
		case asTYPEID_INT32: typeName = "int32"; value = std::to_string(*(int32_t*)ptr); break;
		case asTYPEID_INT64: typeName = "int64"; value = std::to_string(*(int64_t*)ptr); break;

		case asTYPEID_UINT8: typeName = "uint8"; value = std::to_string(*(uint8_t*)ptr); break;
		case asTYPEID_UINT16: typeName = "uint16"; value = std::to_string(*(uint16_t*)ptr); break;
		case asTYPEID_UINT32: typeName = "uint32"; value = std::to_string(*(uint32_t*)ptr); break;
		case asTYPEID_UINT64: typeName = "uint64"; value = std::to_string(*(uint64_t*)ptr); break;

		case asTYPEID_FLOAT: typeName = "float"; value = std::to_string(*(float*)ptr); break;
		case asTYPEID_DOUBLE: typeName = "double"; value = std::to_string(*(double*)ptr); break;

		default:
			if ((typeID & asTYPEID_OBJHANDLE) != 0) {
				typeName += "@";

				asIScriptObject* obj = *(asIScriptObject**)ptr;
				if (obj == nullptr) {
					value = "(null)";
				} else {
					value = StrFormat("%p", ptr);
				}
			} else {
				value = StrFormat("?? (%08X)", typeID);
			}
			break;
		}

		uint16_t lenTypeName = typeName.size();
		sock->SendRaw(&lenTypeName, sizeof(uint16_t));
		sock->SendRaw(typeName.c_str(), lenTypeName);

		uint16_t lenValue = value.size();
		sock->SendRaw(&lenValue, sizeof(uint16_t));
		sock->SendRaw(value.c_str(), lenValue);
	}

	static void Send_AllLocals(EzSock* sock)
	{
		Send_LocalClear(sock);

		int numVars = _ctx->GetVarCount();
		for (int i = 0; i < numVars; i++) {
			const char* varName = _ctx->GetVarName(i);
			int varTypeID = _ctx->GetVarTypeId(i);
			void* varPtr = _ctx->GetAddressOfVar(i);

			Send_Local(sock, varName, varTypeID, varPtr);
		}
	}

	static void ScriptLineCallback(asIScriptContext* ctx)
	{
		if (!_dbgStateBroken) {
			return;
		}

		_clientsMutex.lock();
		for (EzSock* sock : _clientSockets) {
			Send_Location(sock);
			Send_AllLocals(sock);
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

	static void ClientPacket_AddBreakpoint(EzSock* sock)
	{
		//
	}

	static void ClientPacket_DeleteBreakpoint(EzSock* sock)
	{
		//
	}

	static void ClientThreadFunction(EzSock* sock)
	{
		Log("Connection accepted from: %s", inet_ntoa(sock->addr.sin_addr));

		if (_dbgStateBroken) {
			Send_Location(sock);
			Send_AllLocals(sock);
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
			case 4: ClientPacket_AddBreakpoint(sock); break;
			case 5: ClientPacket_DeleteBreakpoint(sock); break;
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

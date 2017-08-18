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
#include <algorithm>

#include <cstdint>

#define EZS_CPP
#include "ezsock.h"

namespace dbg
{
	static asIScriptContext* _ctx = nullptr;

	static std::mutex _logMutex;
	static std::mutex _clientsMutex;
	static std::mutex _breakpointMutex;

	static std::atomic<bool> _initialized = false;
	static std::thread _networkThread;
	static std::vector<std::thread> _clientThreads;
	static std::vector<EzSock*> _clientSockets;

	static std::atomic<bool> _dbgStateBroken = false;

	static std::mutex _dbgStateNotifyStepMutex;
	static std::unique_lock<std::mutex> _dbgStateNotifyStepLock(_dbgStateNotifyStepMutex);
	static std::condition_variable _dbgStateNotifyStep;

	struct Breakpoint
	{
		std::string m_filename;
		int m_line;
	};
	static std::vector<Breakpoint> _dbgStateBreakpoints;

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

	static void Send_Path(EzSock* sock)
	{
		char path[1024];
		GetCurrentDirectoryA(1024, path);

		uint16_t packetType = 4;
		sock->SendRaw(&packetType, sizeof(uint16_t));

		uint16_t lenPath = (uint16_t)strlen(path);
		sock->SendRaw(&lenPath, sizeof(uint16_t));
		sock->SendRaw(path, lenPath);
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

		uint16_t lenTypeName = (uint16_t)typeName.size();
		sock->SendRaw(&lenTypeName, sizeof(uint16_t));
		sock->SendRaw(typeName.c_str(), lenTypeName);

		uint16_t lenValue = (uint16_t)value.size();
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
		_breakpointMutex.lock();
		for (auto &bp : _dbgStateBreakpoints) {
			const char* filename = nullptr;
			int line = ctx->GetLineNumber(0, nullptr, &filename);

			if (bp.m_line == line && bp.m_filename == filename) {
				_dbgStateBroken = true;
				break;
			}
		}
		_breakpointMutex.unlock();

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
		uint16_t lenFilename;
		sock->Receive(&lenFilename, sizeof(uint16_t));

		char* filename = (char*)malloc(lenFilename + 1);
		sock->Receive(filename, lenFilename);
		filename[lenFilename] = '\0';

		int line;
		sock->Receive(&line, sizeof(int));

		_breakpointMutex.lock();
		Breakpoint bp;
		bp.m_filename = filename;
		bp.m_line = line;
		_dbgStateBreakpoints.push_back(bp);
		Log("Added breakpoint at %s line %d", filename, line);
		_breakpointMutex.unlock();

		free(filename);
	}

	static void ClientPacket_DeleteBreakpoint(EzSock* sock)
	{
		uint16_t lenFilename;
		sock->Receive(&lenFilename, sizeof(uint16_t));

		char* filename = (char*)malloc(lenFilename + 1);
		sock->Receive(filename, lenFilename);
		filename[lenFilename] = '\0';

		int line;
		sock->Receive(&line, sizeof(int));

		_breakpointMutex.lock();
		auto it = std::find_if(_dbgStateBreakpoints.begin(), _dbgStateBreakpoints.end(), [filename, line](const Breakpoint &bp) {
			return (bp.m_line == line && bp.m_filename == filename);
		});
		if (it != _dbgStateBreakpoints.end()) {
			Log("Erased breakpoint at %s line %d", it->m_filename.c_str(), it->m_line);
			_dbgStateBreakpoints.erase(it);
		} else {
			Log("Couldn't find any breakpoint at %s line %d", filename, line);
		}
		_breakpointMutex.unlock();

		free(filename);
	}

	static void ClientPacket_SetValue(EzSock* sock)
	{
		uint16_t lenName;
		sock->Receive(&lenName, sizeof(uint16_t));

		char* name = (char*)malloc(lenName + 1);
		sock->Receive(name, lenName);
		name[lenName] = '\0';

		uint16_t lenValue;
		sock->Receive(&lenValue, sizeof(uint16_t));

		char* value = (char*)malloc(lenValue + 1);
		sock->Receive(value, lenValue);
		value[lenValue] = '\0';

		int varIndex = -1;
		int numVars = _ctx->GetVarCount();
		for (int i = 0; i < numVars; i++) {
			const char* varName = _ctx->GetVarName(i);
			if (!strcmp(varName, name)) {
				varIndex = i;
				break;
			}
		}

		if (varIndex != -1) {
			Log("Changing local variable %s to %s", name, value);

			void* varPtr = _ctx->GetAddressOfVar(varIndex);
			int typeID = _ctx->GetVarTypeId(varIndex);

			switch (typeID) {
			case asTYPEID_BOOL: *(bool*)varPtr = (_stricmp(value, "true") == 0); break;

			case asTYPEID_INT8: *(int8_t*)varPtr = std::stoi(value); break;
			case asTYPEID_INT16: *(int16_t*)varPtr = std::stoi(value); break;
			case asTYPEID_INT32: *(int32_t*)varPtr = std::stoi(value); break;
			case asTYPEID_INT64: *(int64_t*)varPtr = std::stoll(value); break;

			case asTYPEID_UINT8: *(uint8_t*)varPtr = (uint8_t)std::stoul(value); break;
			case asTYPEID_UINT16: *(uint16_t*)varPtr = (uint16_t)std::stoul(value); break;
			case asTYPEID_UINT32: *(uint32_t*)varPtr = std::stoul(value); break;
			case asTYPEID_UINT64: *(uint64_t*)varPtr = std::stoull(value); break;

			case asTYPEID_FLOAT: *(float*)varPtr = std::stof(value); break;
			case asTYPEID_DOUBLE: *(double*)varPtr = std::stod(value); break;

			default: /* .. */ break;
			}
		} else {
			Log("Couldn't find local variable %s", name);
		}

		free(name);
		free(value);
	}

	static void ClientThreadFunction(EzSock* sock)
	{
		Log("Connection accepted from: %s", inet_ntoa(sock->addr.sin_addr));

		Send_Path(sock);
		//TODO: Send all existing breakpoints to client

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
			case 6: ClientPacket_SetValue(sock); break;
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

#pragma once

#include <angelscript.h>

#include <string>
#include <functional>

namespace dbg
{
	typedef std::function<std::string(void*)> FuncEncoder;
	typedef std::function<void(void*, const char*)> FuncDecoder;

	void Initialize(asIScriptContext* ctx);
	void Release();

	bool IsInitialized();
	asIScriptContext* GetContext();

	std::string ScriptPath();
	void ScriptPath(const char* path);
	void Encoder(int typeID, FuncEncoder encoder, FuncDecoder decoder);

	void Break();
}

#ifdef ASDBG_IMPL

#include <vector>
#include <iostream>
#include <algorithm>

#include <thread>
#include <atomic>
#include <mutex>
#include <condition_variable>

#include <cstdint>

//////////////////
// Begin of EzSock
// https://github.com/codecat/singleheader-socket
#define _WINSOCKAPI_
#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <sstream>
#include <fcntl.h>
#include <ctype.h>
#include <cstring>

#ifdef _MSC_VER
#include <winsock2.h>
#else
#include <netinet/in.h>
#endif

class AsDbgEzSock
{
public:
	enum SockState
	{
		skDISCONNECTED = 0,
		skUNDEF1, //Not implemented
		skLISTENING,
		skUNDEF3, //Not implemented
		skUNDEF4, //Not implemented
		skUNDEF5, //Not implemented
		skUNDEF6, //Not implemented
		skCONNECTED,
		skERROR
	};

	bool blocking;
	bool Valid;

	struct sockaddr_in addr;
	struct sockaddr_in fromAddr;
	unsigned long fromAddr_len;

	SockState state;

	int lastCode;

	AsDbgEzSock();
	~AsDbgEzSock();

	bool create();
	bool create(int Protocol);
	bool create(int Protocol, int Type);
	bool bind(unsigned short port);
	bool listen();
	bool accept(AsDbgEzSock* socket);
	int connect(const char* host, unsigned short port);
	void close();

	long uAddr();
	bool IsError();

	bool CanRead();

	int sock;
	int Receive(const void* buffer, int size, int spos = 0);
	int SendRaw(const void* data, int dataSize);
	int SendUDP(const void* buffer, int size, sockaddr_in* to);
	int ReceiveUDP(const void* buffer, int size, sockaddr_in* from);

private:
#ifdef _MSC_VER
	WSADATA wsda;
#endif
	int MAXCON;

	fd_set  *scks;
	timeval *times;

	unsigned int totaldata;
	bool check();
};

#ifndef EZSOCK_ALREADY_EXISTS
#ifdef _MSC_VER
#pragma comment(lib,"wsock32.lib")
typedef int socklen_t;
#else
#include <unistd.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <cstring>
#endif

#if !defined(SOCKET_ERROR)
#define SOCKET_ERROR -1
#endif

#if !defined(SOCKET_NONE)
#define SOCKET_NONE 0
#endif

#if !defined(INVALID_SOCKET)
#define INVALID_SOCKET -1
#endif

AsDbgEzSock::AsDbgEzSock()
{
	MAXCON = 64;
	memset(&addr, 0, sizeof(addr));

#ifdef _MSC_VER
	WSAStartup(MAKEWORD(1, 1), &wsda);
#endif

	this->sock = INVALID_SOCKET;
	this->blocking = true;
	this->Valid = false;
	this->scks = new fd_set;
	this->times = new timeval;
	this->times->tv_sec = 0;
	this->times->tv_usec = 0;
	this->state = skDISCONNECTED;
	this->totaldata = 0;
}

AsDbgEzSock::~AsDbgEzSock()
{
	if (this->check())
		close();

	delete scks;
	delete times;
}

bool AsDbgEzSock::check()
{
	return sock > SOCKET_NONE;
}

bool AsDbgEzSock::create()
{
	return this->create(IPPROTO_TCP, SOCK_STREAM);
}

bool AsDbgEzSock::create(int Protocol)
{
	switch (Protocol) {
	case IPPROTO_TCP: return this->create(IPPROTO_TCP, SOCK_STREAM);
	case IPPROTO_UDP: return this->create(IPPROTO_UDP, SOCK_DGRAM);
	default:          return this->create(Protocol, SOCK_RAW);
	}
}

bool AsDbgEzSock::create(int Protocol, int Type)
{
	if (this->check())
		return false;

	state = skDISCONNECTED;
	sock = ::socket(AF_INET, Type, Protocol);
	lastCode = sock;

	return sock > SOCKET_NONE;
}

bool AsDbgEzSock::bind(unsigned short port)
{
	if (!check()) return false;

	addr.sin_family = AF_INET;
	addr.sin_addr.s_addr = htonl(INADDR_ANY);
	addr.sin_port = htons(port);
	lastCode = ::bind(sock, (struct sockaddr*)&addr, sizeof(addr));

	return !lastCode;
}

bool AsDbgEzSock::listen()
{
	lastCode = ::listen(sock, MAXCON);
	if (lastCode == SOCKET_ERROR) return false;

	state = skLISTENING;
	this->Valid = true;
	return true;
}

bool AsDbgEzSock::accept(AsDbgEzSock* socket)
{
	if (!blocking && !CanRead()) return false;

	int length = sizeof(socket->addr);
	socket->sock = ::accept(sock, (struct sockaddr*) &socket->addr, (socklen_t*)&length);

	lastCode = socket->sock;
	if (socket->sock == SOCKET_ERROR)
		return false;

	socket->state = skCONNECTED;
	return true;
}

void AsDbgEzSock::close()
{
	state = skDISCONNECTED;

#ifdef _MSC_VER
	::closesocket(sock);
#else
	::shutdown(sock, SHUT_RDWR);
	::close(sock);
#endif

	sock = INVALID_SOCKET;
}

long AsDbgEzSock::uAddr()
{
	return addr.sin_addr.s_addr;
}

int AsDbgEzSock::connect(const char* host, unsigned short port)
{
	if (!check())
		return 1;

	struct hostent* phe;
	phe = gethostbyname(host);
	if (phe == NULL)
		return 2;

	memcpy(&addr.sin_addr, phe->h_addr, sizeof(struct in_addr));

	addr.sin_family = AF_INET;
	addr.sin_port = htons(port);

	if (::connect(sock, (struct sockaddr*)&addr, sizeof(addr)) == SOCKET_ERROR)
		return 3;

	state = skCONNECTED;
	this->Valid = true;
	return 0;
}

bool AsDbgEzSock::CanRead()
{
	FD_ZERO(scks);
	FD_SET((unsigned)sock, scks);

	return select(sock + 1, scks, NULL, NULL, times) > 0;
}

bool AsDbgEzSock::IsError()
{
	if (state == skERROR || sock == -1)
		return true;

	FD_ZERO(scks);
	FD_SET((unsigned)sock, scks);

	if (select(sock + 1, NULL, NULL, scks, times) >= 0)
		return false;

	state = skERROR;
	return true;
}

int AsDbgEzSock::ReceiveUDP(const void* buffer, int size, sockaddr_in* from)
{
#ifdef _MSC_VER
	int client_length = (int)sizeof(struct sockaddr_in);
#else
	unsigned int client_length = (unsigned int)sizeof(struct sockaddr_in);
#endif
	return recvfrom(this->sock, (char*)buffer, size, 0, (struct sockaddr*)from, &client_length);
}

int AsDbgEzSock::Receive(const void* buffer, int size, int spos)
{
	return recv(this->sock, (char*)buffer + spos, size, 0);
}

int AsDbgEzSock::SendUDP(const void* buffer, int size, sockaddr_in* to)
{
	return sendto(this->sock, (char*)buffer, size, 0, (struct sockaddr *)&to, sizeof(struct sockaddr_in));
}

int AsDbgEzSock::SendRaw(const void* data, int dataSize)
{
	return send(this->sock, (char*)data, dataSize, 0);
}
#endif
// End of EzSock
////////////////

namespace dbg
{
	struct TypeEncoder
	{
		int m_typeID;
		FuncEncoder m_encoder;
		FuncDecoder m_decoder;
	};

	static asIScriptContext* _ctx = nullptr;

	static std::string _scriptPath;

	static std::mutex _logMutex;
	static std::mutex _clientsMutex;
	static std::mutex _breakpointMutex;

	static std::atomic<bool> _initialized;
	static std::thread _networkThread;
	static std::vector<std::thread> _clientThreads;
	static std::vector<AsDbgEzSock*> _clientSockets;

	static std::vector<TypeEncoder> _typeEncoders;

	static std::atomic<bool> _dbgStateBroken;
	static std::atomic<int> _dbgStateBrokenDepth;

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

	static void Send_Path(AsDbgEzSock* sock)
	{
		uint16_t packetType = 4;
		sock->SendRaw(&packetType, sizeof(uint16_t));

		uint16_t lenPath = (uint16_t)_scriptPath.size();
		sock->SendRaw(&lenPath, sizeof(uint16_t));
		sock->SendRaw(_scriptPath.c_str(), lenPath);
	}

	static void Send_Location(AsDbgEzSock* sock)
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

	static void Send_LocalClear(AsDbgEzSock* sock)
	{
		uint16_t packetType = 2;
		sock->SendRaw(&packetType, sizeof(uint16_t));
	}

	static void Send_Local(AsDbgEzSock* sock, const char* name, int typeID, void* ptr)
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
				if (ptr == nullptr) {
					value = "(null)";
				} else {
					bool found = false;
					for (auto &typeEncoder : _typeEncoders) {
						if (typeEncoder.m_typeID == typeID) {
							value = typeEncoder.m_encoder(ptr);
							found = true;
							break;
						}
					}
					if (!found) {
						value = StrFormat("?? (%08X)", typeID);
					}
				}
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

	static void Send_AllLocals(AsDbgEzSock* sock)
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
		// Check breakpoints
		bool breakPointHit = false;
		_breakpointMutex.lock();
		for (auto &bp : _dbgStateBreakpoints) {
			const char* filename = nullptr;
			int line = ctx->GetLineNumber(0, nullptr, &filename);

			if (bp.m_line == line && !strcmp(bp.m_filename.c_str() + _scriptPath.size(), filename)) {
				breakPointHit = true;
				break;
			}
		}
		_breakpointMutex.unlock();

		// If we hit a breakpoint on this line
		if (breakPointHit) {
			// Break now
			_dbgStateBroken = true;
		} else {
			// Otherwise, check if we are stepping over
			if (_dbgStateBrokenDepth != -1) {
				// If we're still a frame ahead
				if ((int)ctx->GetCallstackSize() > _dbgStateBrokenDepth) {
					// Continue execution
					return;
				} else {
					// We've returned on our original frame, we should break now
					_dbgStateBroken = true;
					_dbgStateBrokenDepth = -1;
				}
			}
		}

		// If we're not broken
		if (!_dbgStateBroken) {
			// Continue execution
			return;
		}

		_clientsMutex.lock();
		for (AsDbgEzSock* sock : _clientSockets) {
			Send_Location(sock);
			Send_AllLocals(sock);
		}
		_clientsMutex.unlock();

		// Wait for step notification
		_dbgStateNotifyStep.wait(_dbgStateNotifyStepLock);
	}

	static void ClientPacket_Step(AsDbgEzSock* sock)
	{
		_dbgStateNotifyStep.notify_one();
	}

	static void ClientPacket_StepOver(AsDbgEzSock* sock)
	{
		_dbgStateBrokenDepth = _ctx->GetCallstackSize();
		_dbgStateNotifyStep.notify_one();
	}

	static void ClientPacket_StepOut(AsDbgEzSock* sock)
	{
		if (_ctx->GetCallstackSize() == 1) {
			_dbgStateBroken = false;
			_dbgStateNotifyStep.notify_one();
			return;
		}
		_dbgStateBrokenDepth = _ctx->GetCallstackSize() - 1;
		_dbgStateNotifyStep.notify_one();
	}

	static void ClientPacket_Pause(AsDbgEzSock* sock)
	{
		_dbgStateBroken = true;
	}

	static void ClientPacket_Resume(AsDbgEzSock* sock)
	{
		bool wasBroken = _dbgStateBroken;
		_dbgStateBroken = false;
		if (wasBroken) {
			_dbgStateNotifyStep.notify_one();
		}
	}

	static void ClientPacket_AddBreakpoint(AsDbgEzSock* sock)
	{
		uint16_t lenFilename = 0;
		sock->Receive(&lenFilename, sizeof(uint16_t));

		char* filename = (char*)malloc(lenFilename + 1);
		sock->Receive(filename, lenFilename);
		filename[lenFilename] = '\0';

		int line = 0;
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

	static void ClientPacket_DeleteBreakpoint(AsDbgEzSock* sock)
	{
		uint16_t lenFilename = 0;
		sock->Receive(&lenFilename, sizeof(uint16_t));

		char* filename = (char*)malloc(lenFilename + 1);
		sock->Receive(filename, lenFilename);
		filename[lenFilename] = '\0';

		int line = 0;
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

	static void ClientPacket_SetValue(AsDbgEzSock* sock)
	{
		uint16_t lenName = 0;
		sock->Receive(&lenName, sizeof(uint16_t));

		char* name = (char*)malloc(lenName + 1);
		sock->Receive(name, lenName);
		name[lenName] = '\0';

		uint16_t lenValue = 0;
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
			case asTYPEID_BOOL: *(bool*)varPtr = (strcmp(value, "true") == 0); break;

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

			default:
				bool found = false;
				for (auto &typeEncoder : _typeEncoders) {
					if (typeEncoder.m_typeID == typeID) {
						typeEncoder.m_decoder(varPtr, value);
						found = true;
						break;
					}
				}
				if (!found) {
					Log("Couldn't find decoder for type %08x", typeID);
				}
				break;
			}
		} else {
			Log("Couldn't find local variable %s", name);
		}

		free(name);
		free(value);
	}

	static void ClientThreadFunction(AsDbgEzSock* sock)
	{
		Log("Connection accepted from: %s", inet_ntoa(sock->addr.sin_addr));

		Send_Path(sock);
		//TODO: Send all existing breakpoints to client

		if (_dbgStateBroken) {
			Send_Location(sock);
			Send_AllLocals(sock);
		}

		while (_initialized) {
			uint16_t packetType = 0;
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
			case 7: ClientPacket_StepOver(sock); break;
			case 8: ClientPacket_StepOut(sock); break;
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

		AsDbgEzSock sock;
		sock.create();
		sock.bind(sockPort);
		if (!sock.listen()) {
			Log("Failed to listen on port %d", sockPort);
			return;
		}
		Log("Socket listening on port %d", sockPort);

		while (_initialized) {
			AsDbgEzSock* client = new AsDbgEzSock;
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

#ifdef _MSC_VER
		char path[1024];
		GetCurrentDirectoryA(1024, path);
		_scriptPath = path;
#else
		//TODO
#endif

		_dbgStateBrokenDepth = -1;

		_initialized = true;
		_networkThread = std::thread(NetworkThreadFunction);
		_typeEncoders.clear();

		Log("Debugger initialized");
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

		_ctx->ClearLineCallback();
		_ctx = nullptr;

		Log("Debugger released");
	}

	bool IsInitialized()
	{
		return _initialized;
	}

	asIScriptContext* GetContext()
	{
		return _ctx;
	}

	std::string ScriptPath()
	{
		return _scriptPath;
	}

	void ScriptPath(const char* path)
	{
		_scriptPath = path;
	}

	void Encoder(int typeID, FuncEncoder encoder, FuncDecoder decoder)
	{
		TypeEncoder typeEncoder;
		typeEncoder.m_typeID = typeID;
		typeEncoder.m_encoder = encoder;
		typeEncoder.m_decoder = decoder;
		_typeEncoders.push_back(typeEncoder);
	}

	void Break()
	{
		_dbgStateBroken = true;
	}
}

#endif

#ifndef __EzSock_H__
#define __EzSock_H__

#define _WINSOCKAPI_
#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <sstream>
#include <vector>
#include <fcntl.h>
#include <ctype.h>

#ifdef _MSC_VER
#include <winsock2.h>
#else
#include <netinet/in.h>
#endif

class EzSock
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

	EzSock();
	~EzSock();

	bool create();
	bool create(int Protocol);
	bool create(int Protocol, int Type);
	bool bind(unsigned short port);
	bool listen();
	bool accept(EzSock* socket);
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

#endif

#ifdef EZS_CPP
#ifndef __EzSock_CPP__
#define __EzSock_CPP__

#include <iostream>

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

EzSock::EzSock()
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

EzSock::~EzSock()
{
	if (this->check())
		close();

	delete scks;
	delete times;
}

bool EzSock::check()
{
	return sock > SOCKET_NONE;
}

bool EzSock::create()
{
	return this->create(IPPROTO_TCP, SOCK_STREAM);
}

bool EzSock::create(int Protocol)
{
	switch (Protocol) {
	case IPPROTO_TCP: return this->create(IPPROTO_TCP, SOCK_STREAM);
	case IPPROTO_UDP: return this->create(IPPROTO_UDP, SOCK_DGRAM);
	default:          return this->create(Protocol, SOCK_RAW);
	}
}

bool EzSock::create(int Protocol, int Type)
{
	if (this->check())
		return false;

	state = skDISCONNECTED;
	sock = ::socket(AF_INET, Type, Protocol);
	lastCode = sock;

	return sock > SOCKET_NONE;
}

bool EzSock::bind(unsigned short port)
{
	if (!check()) return false;

	addr.sin_family = AF_INET;
	addr.sin_addr.s_addr = htonl(INADDR_ANY);
	addr.sin_port = htons(port);
	lastCode = ::bind(sock, (struct sockaddr*)&addr, sizeof(addr));

	return !lastCode;
}

bool EzSock::listen()
{
	lastCode = ::listen(sock, MAXCON);
	if (lastCode == SOCKET_ERROR) return false;

	state = skLISTENING;
	this->Valid = true;
	return true;
}

bool EzSock::accept(EzSock* socket)
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

void EzSock::close()
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

long EzSock::uAddr()
{
	return addr.sin_addr.s_addr;
}

int EzSock::connect(const char* host, unsigned short port)
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

bool EzSock::CanRead()
{
	FD_ZERO(scks);
	FD_SET((unsigned)sock, scks);

	return select(sock + 1, scks, NULL, NULL, times) > 0;
}

bool EzSock::IsError()
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

int EzSock::ReceiveUDP(const void* buffer, int size, sockaddr_in* from)
{
#ifdef _MSC_VER
	int client_length = (int)sizeof(struct sockaddr_in);
#else
	unsigned int client_length = (unsigned int)sizeof(struct sockaddr_in);
#endif
	return recvfrom(this->sock, (char*)buffer, size, 0, (struct sockaddr*)from, &client_length);
}

int EzSock::Receive(const void* buffer, int size, int spos)
{
	return recv(this->sock, (char*)buffer + spos, size, 0);
}

int EzSock::SendUDP(const void* buffer, int size, sockaddr_in* to)
{
	return sendto(this->sock, (char*)buffer, size, 0, (struct sockaddr *)&to, sizeof(struct sockaddr_in));
}

int EzSock::SendRaw(const void* data, int dataSize)
{
	return send(this->sock, (char*)data, dataSize, 0);
}

#endif
#endif

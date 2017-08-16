#include <cstdio>

#include <angelscript.h>

#include "addons/scriptstdstring/scriptstdstring.h"
#include "addons/scriptarray/scriptarray.h"
#include "addons/scriptdictionary/scriptdictionary.h"
#include "addons/scripthandle/scripthandle.h"

#include "addons/scriptbuilder/scriptbuilder.h"

#define ASDBG_IMPL
#include "asdbg.h"

static void MessageCallback(const asSMessageInfo* msg, void* param)
{
	const char* type = "";
	switch (msg->type) {
	case asMSGTYPE_INFORMATION: type = "INFO"; break;
	case asMSGTYPE_WARNING: type = "WARN"; break;
	case asMSGTYPE_ERROR: type = "ERROR"; break;
	}
	printf("[%s] (%s line %d column %d) %s\n", type, msg->section, msg->row, msg->col, msg->message);
}

static void ScriptPrint(const std::string &str)
{
	printf("%s\n", str.c_str());
}

int main()
{
	asIScriptEngine* engine = asCreateScriptEngine();

	engine->SetMessageCallback(asFUNCTION(MessageCallback), nullptr, asCALL_CDECL);

	RegisterStdString(engine);
	RegisterScriptArray(engine, true);
	RegisterScriptDictionary(engine);
	RegisterScriptHandle(engine);

	RegisterStdStringUtils(engine);

	engine->RegisterGlobalFunction("void print(const string &in)", asFUNCTION(ScriptPrint), asCALL_CDECL);
	engine->RegisterGlobalFunction("void dbg_break()", asFUNCTION(dbg::Break), asCALL_CDECL);

	CScriptBuilder builder;
	builder.StartNewModule(engine, "Scripts");
	builder.AddSectionFromFile("test.as");
	if (builder.BuildModule() != asSUCCESS) {
		printf("Scripts failed to compile!\n");
		getchar();
		return 1;
	}

	asIScriptFunction* func = builder.GetModule()->GetFunctionByDecl("void main()");
	asIScriptContext* ctx = engine->CreateContext();

	dbg::Initialize(ctx);
	dbg::Break();

	ctx->Prepare(func);
	ctx->Execute();

	dbg::Release();

	ctx->Release();

	engine->Release();

	printf("The end.\n");
	getchar();

	return 0;
}

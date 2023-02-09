#pragma once

#ifdef SOARUNITYAPI_EXPORTS
#define SOARUNITYAPI_API __declspec(dllexport)
#else
#define SOARUNITYAPI_API __declspec(dllimport)
#endif

extern "C" {
	//######## Manage Kernel & Agent & Productions #######################
	SOARUNITYAPI_API sml::Kernel* createKernelInNewThread();

	SOARUNITYAPI_API sml::Agent* createAgent(const char*, sml::Kernel*);

	SOARUNITYAPI_API int loadProductions(sml::Agent*, const char*, bool);

	//##################### Manage IO ######################
	SOARUNITYAPI_API sml::Identifier* getInputLink(sml::Agent*);

	SOARUNITYAPI_API sml::Identifier* getOutputLink(sml::Agent* pAgent);

	//##################### Manage WMEs ######################
	SOARUNITYAPI_API sml::StringElement* createStringWME(sml::Agent*, sml::Identifier*, const char*, const char*);
	
	SOARUNITYAPI_API sml::IntElement* createIntWME(sml::Agent*, sml::Identifier*, const char*, long long);
	
	SOARUNITYAPI_API sml::FloatElement* createFloatWME(sml::Agent*, sml::Identifier*, const char*, double);

	SOARUNITYAPI_API sml::Identifier* createIdWME(sml::Agent*, sml::Identifier*, const char*);
	
	SOARUNITYAPI_API sml::Identifier* createSharedIdWME(sml::Agent*, sml::Identifier*, char const*, sml::Identifier*);
	
	SOARUNITYAPI_API void updateStringWME(sml::Agent*, sml::StringElement*, char const*);

	SOARUNITYAPI_API void updateIntWME(sml::Agent*, sml::IntElement*, long long);

	SOARUNITYAPI_API void updateFloatWME(sml::Agent*, sml::FloatElement*, double);

	SOARUNITYAPI_API void setBlinkIfNoChange(sml::Agent*, bool);

	SOARUNITYAPI_API bool isBlinkIfNoChange(sml::Agent*);

	SOARUNITYAPI_API bool destroyWME(sml::Agent*, sml::WMElement*);

	SOARUNITYAPI_API char const* initSoar(sml::Agent*);

	SOARUNITYAPI_API void setOutputLinkChangeTracking(sml::Agent*, bool);

	SOARUNITYAPI_API int getNumberOutputLinkChanges(sml::Agent* );

	SOARUNITYAPI_API sml::WMElement* getOutputLinkChange(sml::Agent*, int);

	SOARUNITYAPI_API bool isOutputLinkChangeAdd(sml::Agent*, int);

	SOARUNITYAPI_API int getNumberCommands(sml::Agent*);

	SOARUNITYAPI_API bool commands(sml::Agent*);

	SOARUNITYAPI_API sml::Identifier* getCommand(sml::Agent*, int );

	SOARUNITYAPI_API void commit(sml::Agent*);

	SOARUNITYAPI_API bool isCommitRequired(sml::Agent*);

	SOARUNITYAPI_API void setAutoCommit(sml::Kernel*, bool);

	//##################### Identifier ######################
	SOARUNITYAPI_API char const* getCommandName(sml::Identifier*);

	SOARUNITYAPI_API void addStatusComplete(sml::Identifier*);

	SOARUNITYAPI_API void addStatusError(sml::Identifier*);

	//##################### Events ######################
	//###### Print
	SOARUNITYAPI_API int registerForPrintEvent(sml::Agent*, sml::smlPrintEventId, sml::PrintEventHandler, void*, bool, bool);

	SOARUNITYAPI_API bool unregisterForPrintEvent(sml::Agent*, int);

	//###### Update
	SOARUNITYAPI_API int registerForUpdateEvent(sml::Kernel*, sml::smlUpdateEventId, sml::UpdateEventHandler, void*, bool);

	SOARUNITYAPI_API bool unregisterForUpdateEvent(sml::Kernel*, int);

	//##################### Run Agent ######################
	SOARUNITYAPI_API char const* runSelf(sml::Agent*, int, sml::smlRunStepSize);

	SOARUNITYAPI_API void runSelfForever(sml::Agent*);

	SOARUNITYAPI_API void runSelfTilOutput(sml::Agent*);

	//##################### Debug ######################
	//SOARUNITYAPI_API bool spawnDebugger(sml::Agent*, int, const char*);

	//SOARUNITYAPI_API bool killDebugger(sml::Agent*);
}
#pragma once

#ifdef SOARUNITYAPI_EXPORTS
#define SOARUNITYAPI_API __declspec(dllexport)
#else
#define SOARUNITYAPI_API __declspec(dllimport)
#endif

extern "C" {
	//######## Manage Kernel & Agent & Productions #######################
	SOARUNITYAPI_API sml::Kernel* createKernel();

	SOARUNITYAPI_API sml::Agent* createAgent(const char*, sml::Kernel*);

	SOARUNITYAPI_API int loadProductions(sml::Agent*, const char*);

	//##################### Manage IO ######################
	SOARUNITYAPI_API sml::Identifier* getInputLink(sml::Agent*);

	//##################### Manage WMEs ######################
	SOARUNITYAPI_API sml::Identifier* createIdWME(sml::Agent*, sml::Identifier*, const char*);

	SOARUNITYAPI_API sml::StringElement* createStringWME(sml::Agent*, sml::Identifier*, const char*, const char*);
	
	SOARUNITYAPI_API sml::IntElement* createIntWME(sml::Agent*, sml::Identifier*, const char*, long long);
	
	SOARUNITYAPI_API sml::FloatElement* createFloatWME(sml::Agent*, sml::Identifier*, const char*, double);
	
	SOARUNITYAPI_API void commit(sml::Agent*);

	//##################### Run Agent ######################
	SOARUNITYAPI_API void runSelfTilOutput(sml::Agent*);

	//##################### Events ######################
	SOARUNITYAPI_API int registerForPrintEvent(sml::Agent*);
	SOARUNITYAPI_API int registerForProductionAddedEvent(sml::Agent*);
}
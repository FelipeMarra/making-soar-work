#pragma once

#ifdef SOARUNITYAPI_EXPORTS
#define SOARUNITYAPI_API __declspec(dllexport)
#else
#define SOARUNITYAPI_API __declspec(dllimport)
#endif

extern "C" {
	SOARUNITYAPI_API sml::Kernel* createSoarKernel();

	SOARUNITYAPI_API sml::Agent* createSoarAgent(const char*, sml::Kernel*);

	SOARUNITYAPI_API int loadSoarProductions(sml::Agent*, const char*);
}
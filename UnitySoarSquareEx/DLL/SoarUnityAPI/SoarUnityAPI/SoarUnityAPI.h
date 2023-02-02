#pragma once

#ifdef SOARUNITYAPI_EXPORTS
#define SOARUNITYAPI_API __declspec(dllexport)
#else
#define SOARUNITYAPI_API __declspec(dllimport)
#endif

extern "C" {
	SOARUNITYAPI_API int createSoarKernel();

	SOARUNITYAPI_API int createSoarAgent(const char*);
}
#pragma once

#ifdef SOARCPP_EXPORTS
#define SOARCPP_API __declspec(dllexport)
#else
#define SOARCPP_API __declspec(dllimport)
#endif

extern "C" SOARCPP_API void soar_test();
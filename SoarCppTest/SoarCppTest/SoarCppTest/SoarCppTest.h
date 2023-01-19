#pragma once

#ifdef SOARCPPTEST_EXPORTS
#define SOARCPPTEST_API __declspec(dllexport)
#else
#define SOARCPPTEST_API __declspec(dllimport)
#endif

extern "C" SOARCPPTEST_API int soar_test();
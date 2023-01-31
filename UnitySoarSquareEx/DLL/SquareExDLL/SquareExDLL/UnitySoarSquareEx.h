#pragma once

#ifdef UNITYSOARSQUAREEX_EXPORTS
#define UNITYSOARSQUAREEX_API __declspec(dllexport)
#else
#define UNITYSOARSQUAREEX_API __declspec(dllimport)
#endif

extern "C" UNITYSOARSQUAREEX_API int soar_test();

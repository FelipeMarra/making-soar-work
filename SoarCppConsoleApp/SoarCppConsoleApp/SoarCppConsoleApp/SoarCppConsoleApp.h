#pragma once

#ifndef SOAR_CPP_CONSOLE_APP_H
#define SOAR_CPP_CONSOLE_APP_H

#define DLLExport __declspec(dllexport)

extern "C"
{
    DLLExport int soar_test();
}
#endif
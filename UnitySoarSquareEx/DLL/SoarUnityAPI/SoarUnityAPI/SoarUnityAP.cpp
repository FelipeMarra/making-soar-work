#include "pch.h"
#include <iostream>
//Soar
#include "include/sml_Client.h"
//utils
#include "UnityDebug.h"
//h
#include "SoarUnityAPI.h"

using namespace sml;
using namespace std;

Kernel* pKernel;
sml::Agent* pAgent;

///<summary>
/// @brief Prints a message plus the kernel last error description into Unity's console
///</summary>
void printKernelLastError(const char* message) {
    char errorMessage[100];
    strcat_s(errorMessage, message);
    strcat_s(errorMessage, pKernel->GetLastErrorDescription());
    Debug::Log(errorMessage, Color::Red);
}

///<summary>
/// @brief Initializes Soar by creating a kernel in a new thread
/// 
/// @returns  -1 for errors and 0 otherwise
///</summary>
int createSoarKernel() {
    // Create an instance of the Soar kernel in our process
    pKernel = Kernel::CreateKernelInNewThread();

    // Check that nothing went wrong.  We will always get back a kernel object
    // even if something went wrong and we have to abort.
    if (pKernel->HadError()) {
        printKernelLastError("createSoarKernel: ");
        return -1;
    }

    Debug::Log("createSoarKernel: Kernel Created", Color::Green);
    return 0;
}

///<summary>
/// @brief Initializes Soar by creating a kernel in a new trhread and
/// @returns -2 for error and 0 otherwise
///</summary>
int createSoarAgent(const char* name) {
    // Create a Soar agent named "square"
    // NOTE: We don't delete the agent pointer.  It's owned by the kernel
    pAgent = pKernel->CreateAgent(name);

    // Check that nothing went wrong
    // NOTE: No agent gets created if there's a problem, so we have to check for
    // errors through the kernel object.
    if (pKernel->HadError()) {
        printKernelLastError("createSoarAgent: ");
        return -1;
    }

    Debug::Log("createSoarAgent: Agent Created", Color::Green);
    return 0;
}
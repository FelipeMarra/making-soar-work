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

///<summary>
/// @brief Prints a message plus the kernel last error description into Unity's console
///</summary>
void printKernelLastError(Kernel* pKernel, const char* message = "") {
    char errorMessage[100];
    strcat_s(errorMessage, message);
    strcat_s(errorMessage, pKernel->GetLastErrorDescription());
    Debug::Log(errorMessage, Color::Red);
}

///<summary>
/// @brief Initializes Soar by creating a kernel in a new thread
/// 
/// @returns  NULL for errors and Kernel* otherwise
///</summary>
sml::Kernel* createSoarKernel() {
    // Create an instance of the Soar kernel in our process
    sml::Kernel* pKernel = Kernel::CreateKernelInNewThread();

    // Check that nothing went wrong.  We will always get back a kernel object
    // even if something went wrong and we have to abort.
    if (pKernel->HadError()) {
        printKernelLastError(pKernel, "createSoarKernel: ");
        return NULL;
    }

    Debug::Log("createSoarKernel: Kernel Created", Color::Green);
    return pKernel;
}

///<summary>
/// @brief Initializes Soar by creating a kernel in a new trhread and
/// @returns NULL for error and sml::Agent* otherwise
///</summary>
sml::Agent* createSoarAgent(const char* name, Kernel* pKernel) {
    // Create a Soar agent named "square"
    // NOTE: We don't delete the agent pointer.  It's owned by the kernel
    sml::Agent* pAgent = pKernel->CreateAgent(name);

    // Check that nothing went wrong
    // NOTE: No agent gets created if there's a problem, so we have to check for
    // errors through the kernel object.
    if (pKernel->HadError()) {
        printKernelLastError(pKernel, "createSoarAgent: ");
        return NULL;
    }

    Debug::Log("createSoarAgent: Agent Created", Color::Green);
    return pAgent;
}
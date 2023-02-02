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

/*************************************************************
*@brief Prints a message plus the kernel last error description into Unity's console
* 
* @param pKernel: The kernel that will be used to get the error
* 
* @param message: Optional message shown before the kernel error
*************************************************************/
void printKernelLastError(Kernel* pKernel, const char* message = "") {
    char errorMessage[100];
    strcat_s(errorMessage, message);
    strcat_s(errorMessage, pKernel->GetLastErrorDescription());
    Debug::Log(errorMessage, Color::Red);
}


/*************************************************************
*@brief Initializes Soar by creating a kernel in a new thread.
*
*@returns A new kernel object which is used to communicate with the kernel (or NULL if an error occured).
*************************************************************/
sml::Kernel* createSoarKernel() {
    // Create an instance of the Soar kernel in our process
    sml::Kernel* pKernel = Kernel::CreateKernelInNewThread();

    // Check that nothing went wrong. We will always get back a kernel object
    // even if something went wrong and we have to abort.
    if (pKernel->HadError()) {
        printKernelLastError(pKernel, "createSoarKernel: ");
        return NULL;
    }

    Debug::Log("createSoarKernel: Kernel Created", Color::Green);
    return pKernel;
}

/*************************************************************
*@brief Creates a agent inside the specified kernel, with the specified name
*
* @param name: The agent's name
* 
* @param pKernel: The kernel that will be used to create the agent
* 
*@returns A pointer to the agent(or NULL if not found).This object
* is owned by the kernela will be destroyed when the kernel is destroyed.
*************************************************************/
sml::Agent* createSoarAgent(const char* name, Kernel* pKernel) {
    sml::Agent* pAgent = pKernel->CreateAgent(name);

    // NOTE: No agent gets created if there's a problem, so we have to check for
    // errors through the kernel object.
    if (pKernel->HadError()) {
        printKernelLastError(pKernel, "createSoarAgent: ");
        return NULL;
    }

    Debug::Log("createSoarAgent: Agent Created", Color::Green);
    return pAgent;
}
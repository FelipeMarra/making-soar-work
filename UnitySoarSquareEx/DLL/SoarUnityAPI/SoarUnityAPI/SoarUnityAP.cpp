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
*@brief Prints a potMessage plus the last error description 
* (use pKernel or pAgent ->GetLastErrorDescription) into Unity's console
* 
* @param pKernel: The kernel that will be used to get the error
* 
* @param message: Optional message shown before the error
*************************************************************/
void printError(const char* soarMessage, const char* optMessage = "") {
    char errorMessage[150];
    strcat_s(errorMessage, optMessage);
    strcat_s(errorMessage, soarMessage);
    
    //cout << errorMessage<<endl;
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
        printError(pKernel->GetLastErrorDescription(), "createSoarKernel: ");
        return NULL;
    }

    //cout << "createSoarKernel: Kernel Created" << endl;
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
sml::Agent* createSoarAgent(const char* name, sml::Kernel* pKernel) {
    sml::Agent* pAgent = pKernel->CreateAgent(name);

    // NOTE: No agent gets created if there's a problem, so we have to check for
    // errors through the kernel object.
    if (pKernel->HadError()) {
        printError(pKernel->GetLastErrorDescription(), "createSoarAgent: ");
        return NULL;
    }

    //cout << "createSoarAgent: Agent Created" << endl;
    Debug::Log("createSoarAgent: Agent Created", Color::Green);
    return pAgent;
}

/*************************************************************
*@brief Loads Soar producitons from file into agnet
*
* @param pAgent: The agent's pointer
*
* @param pKernel: The kernel's pointer for debuging
* 
* @param path: The productions file location
*
*@returns -1 for error and 0 otherwise
*************************************************************/
int loadSoarProductions(sml::Agent* pAgent, const char* path) {
    pAgent->LoadProductions(path);

   if (pAgent->HadError()) {
       printError(pAgent->GetLastErrorDescription(), "loadSoarProductions: ");
       return -1;
    }

    //cout << message << endl;
    Debug::Log("loadSoarProductions: Loaded Productions", Color::Green);
    return 0;
}

//int main() {
//    sml::Kernel* pKernel = createSoarKernel();
//    sml::Agent* pAgent = createSoarAgent("teste", pKernel);
//    loadSoarProductions(pAgent, "move-north.soar");
//}
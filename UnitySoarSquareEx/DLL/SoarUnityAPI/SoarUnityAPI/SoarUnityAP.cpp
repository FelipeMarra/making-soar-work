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

//######## Manage Kernel & Agent & Productions #######################
#pragma region Initialization

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
sml::Kernel* createKernel() {
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
sml::Agent* createAgent(const char* name, sml::Kernel* pKernel) {
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
* @param path: The productions file location
*
*@returns -1 for error and 0 otherwise
*************************************************************/
int loadProductions(sml::Agent* pAgent, const char* path) {
    bool loaded = pAgent->LoadProductions(path);

   if (!loaded || pAgent->HadError()) {
       printError(pAgent->GetLastErrorDescription(), "loadSoarProductions: ");
       return -1;
    }

    //cout << "loadSoarProductions: Loaded Productions" << endl;
    Debug::Log("loadSoarProductions: Loaded Productions", Color::Green);
    return 0;
}

#pragma endregion

//##################### Manage IO ######################
#pragma region IO

/*************************************************************
*@brief Get the agent's io.input-link identifier
*
* @param pAgent: The agent's pointer
*
*@returns A pointer (Identifier*) to the identifier of the input-link
*************************************************************/
sml::Identifier* getInputLink(sml::Agent* pAgent) {
   return pAgent->GetInputLink();
}

#pragma endregion

//##################### Manage WMEs ######################
#pragma region WMEs

/*************************************************************
*@brief Create a string WME [pAtribute] with a identifier as the value. 
* Output note: the agent can move it anytime, so may cause seg fault to use the [Identifier*]
*
*@param pAgent: The agent's pointer
*
*@returns A pointer [Identifier*] to the identifier of the input-link
*************************************************************/
sml::Identifier* createIdWME(sml::Agent* pAgent, sml::Identifier* parent, const char* pAtribute) {
    return pAgent->CreateIdWME(parent, pAtribute);
}

sml::StringElement* createStringWME(sml::Agent* pAgent, sml::Identifier* parent, const char* pAtribute, const char* pValue) {
    return pAgent->CreateStringWME(parent, pAtribute, pValue);
}

sml::IntElement* createIntWME(sml::Agent* pAgent, sml::Identifier* parent, const char* pAtribute, long long pValue) {
    return pAgent->CreateIntWME(parent, pAtribute, pValue);
}

sml::FloatElement* createFloatWME(sml::Agent* pAgent, sml::Identifier* parent, const char* pAtribute, double pValue) {
    return pAgent->CreateFloatWME(parent, pAtribute, pValue);
}

void commit(sml::Agent* pAgent) {
    pAgent->Commit();
}

void setAutoCommit(sml::Kernel* pKernel, bool state) {
    pKernel->SetAutoCommit(state);
}

#pragma endregion

//##################### Run Agent ######################
#pragma region Run

/*************************************************************
*@brief Run Soar until it generates output or 15 decision cycles have passed
*
* @param pAgent: The agent's pointer
*************************************************************/
void runSelfTilOutput(sml::Agent* pAgent) {
    pAgent->RunSelfTilOutput();
}

void runSelfForever(sml::Agent* pAgent) {
    pAgent->RunSelfForever();
    //cout << "Agent Running" << endl;
}

#pragma endregion

//##################### Events ######################
#pragma region Events
// TODO: only create register events functions here and use the C# sml example to write the calls inside Unity

//###### Print
/*************************************************************
*@brief Register for an "PrintEvent".
*
* @param pAgent: The agent's pointer
* 
* @returns A unique ID for this callback (used to unregister the callback later)
*************************************************************/
int registerForPrintEvent(sml::Agent* pAgent, sml::smlPrintEventId id, sml::PrintEventHandler handler, void* pUserData, bool ignoreOwnEchos = true, bool addToBack = true) {
    return pAgent->RegisterForPrintEvent(id, handler, pUserData, ignoreOwnEchos, addToBack);
}

//###### Production: TODO smlEVENT_AFTER_PRODUCTION_ADDED DONT WORK
//void productionAddedEventHandler(sml::smlProductionEventId id, void* pUserData, sml::Agent* pAgent, char const* pProdName, char const* pInstantion) {
//    // In this case the user data is a string we're building up
//    std::string* pTrace = (std::string*)pUserData;
//
//    (*pTrace) += pProdName;
//
//    //cout << "productionAddedEventHandler: " << *pTrace << endl;
//    Debug::Log(*pTrace, Color::White);
//}
// 
////NOT WORKING:
//int registerForProductionAddedEvent(sml::Agent* pAgent) {
//    std::string trace = "PROD EVENT: ";
//    //smlEVENT_AFTER_PRODUCTION_ADDED DONT WORK
//    return pAgent->RegisterForProductionEvent(sml::smlEVENT_AFTER_PRODUCTION_ADDED, productionAddedEventHandler, &trace);
//}

#pragma endregion

//##################### Debug ######################
#pragma region Debug
//NOT WORKING:
//bool spawnDebugger(sml::Agent* pAgent, int port, const char* jarpath) {
//    if (port == -1) {
//        port = sml::Kernel::kDefaultSMLPort;
//    }
//
//    Debug::Log(port, Color::Blue);
//    Debug::Log(jarpath, Color::Blue);
//
//    //cout << port<< " " << jarpath <<endl;
//
//    return pAgent->SpawnDebugger(port=port, jarpath=jarpath);
//}

//bool killDebugger(sml::Agent* pAgent) {
//    return pAgent->KillDebugger();
//}
#pragma endregion

//##################### Test ######################
#pragma region Test
//Identifier* inputId;
//
//void runEventHandler(smlRunEventId id, void* pUserData, Agent* pAgent, smlPhase phase) {
//    char cmd[20];
//    strcpy_s(cmd, "print --depth 3 ");
//    strcat_s(cmd, inputId->GetIdentifierName());
//
//    //cout << cmd << endl;
//    const char* answer = pAgent->ExecuteCommandLine(cmd);
//    //cout << answer << endl;
//}
//
//int main(int argc, char* argv[]) {
//    sml::Kernel* pKernel = createKernel();
//    sml::Agent* pAgent = createAgent("teste", pKernel);
//
//    //NOT WORKING:
//    //spawnDebugger(pAgent, -1, "C:\\Users\\felip\\Desktop\\SOAR\\SoarTutorial_9.6.1-Multiplatform\\bin\\SoarJavaDebugger.jar");
//    
//    //NOT WORKING:
//    //registerForProductionAddedEvent(pAgent);
//
//    registerForPrintEvent(pAgent);
//
//    inputId = getInputLink(pAgent);
//    Identifier* squareId = createIdWME(pAgent, inputId, "square");
//    Identifier* positionId = createIdWME(pAgent, squareId, "position");
//    FloatElement* xId = createFloatWME(pAgent, positionId, "x", 1.5);
//    FloatElement* yId = createFloatWME(pAgent, positionId, "y", 1.5);
//    commit(pAgent);
//
//    loadProductions(pAgent, "C:\\Users\\felip\\Documents\\GitHub\\making-soar-work\\UnitySoarSquareEx\\DLL\\SoarUnityAPI\\x64\\Release\\initialize-square-agent.soar");
//    loadProductions(pAgent, "C:\\Users\\felip\\Documents\\GitHub\\making-soar-work\\UnitySoarSquareEx\\DLL\\SoarUnityAPI\\x64\\Release\\move-north.soar");
//    
//    //std::string trace = "RUN EVENT: ";
//    //pAgent->RegisterForRunEvent(smlEVENT_BEFORE_DECISION_CYCLE, runEventHandler, &trace);
//
//    runSelfForever(pAgent);
//
//    std::cin.ignore();
//}
#pragma endregion
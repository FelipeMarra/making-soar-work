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

void printError(const char* soarMessage, const char* optMessage = "") {
    char errorMessage[150];
    strcat_s(errorMessage, optMessage);
    strcat_s(errorMessage, soarMessage);
    
    //cout << errorMessage<<endl;
    Debug::Log(errorMessage, Color::Red);
}

sml::Kernel* createKernelInNewThread() {
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

void shutdown(sml::Kernel* pKernel) {
    pKernel->Shutdown();
}

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

int loadProductions(sml::Agent* pAgent, const char* path, bool echoResults) {
    bool loaded = pAgent->LoadProductions(path, echoResults);

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
sml::Identifier* getInputLink(sml::Agent* pAgent) {
   return pAgent->GetInputLink();
}

sml::Identifier* getOutputLink(sml::Agent* pAgent) {
    return pAgent->GetOutputLink();
}

#pragma endregion

//##################### Manage WMEs ######################
#pragma region WMEs

sml::StringElement* createStringWME(sml::Agent* pAgent, sml::Identifier* parent, const char* pAttribute, const char* pValue) {
    return pAgent->CreateStringWME(parent, pAttribute, pValue);
}

sml::IntElement* createIntWME(sml::Agent* pAgent, sml::Identifier* parent, const char* pAttribute, long long value) {
    return pAgent->CreateIntWME(parent, pAttribute, value);
}

sml::FloatElement* createFloatWME(sml::Agent* pAgent, sml::Identifier* parent, const char* pAttribute, double value) {
    return pAgent->CreateFloatWME(parent, pAttribute, value);
}

sml::Identifier* createIdWME(sml::Agent* pAgent, sml::Identifier* parent, const char* pAttribute) {
    return pAgent->CreateIdWME(parent, pAttribute);
}

sml::Identifier* createSharedIdWME(sml::Agent* pAgent, sml::Identifier* parent, char const* pAttribute, sml::Identifier* pSharedValue) {
    return pAgent->CreateSharedIdWME(parent, pAttribute, pSharedValue);
}

void updateStringWME(sml::Agent* pAgent, sml::StringElement* pWME, char const* pValue) {
    pAgent->Update(pWME, pValue);
}

void updateIntWME(sml::Agent* pAgent, sml::IntElement* pWME, long long value) {
    pAgent->Update(pWME, value);
}

void updateFloatWME(sml::Agent* pAgent, sml::FloatElement* pWME, double value) {
    pAgent->Update(pWME, value);
}

void setBlinkIfNoChange(sml::Agent* pAgent, bool state) {
    pAgent->SetBlinkIfNoChange(state);
}

bool isBlinkIfNoChange(sml::Agent* pAgent) {
    return pAgent->IsBlinkIfNoChange();
}

bool destroyWME(sml::Agent* pAgent, sml::WMElement* pWME) {
     return pAgent->DestroyWME(pWME);
}

const char* initSoar(sml::Agent* pAgent) {
    return pAgent->InitSoar();
}

void setOutputLinkChangeTracking(sml::Agent* pAgent, bool setting) {
    return pAgent->SetOutputLinkChangeTracking(setting);
}

int getNumberOutputLinkChanges(sml::Agent* pAgent) {
    return pAgent->GetNumberOutputLinkChanges();
}

sml::WMElement* getOutputLinkChange(sml::Agent* pAgent, int index) {
    return pAgent->GetOutputLinkChange(index);
}

bool isOutputLinkChangeAdd(sml::Agent* pAgent, int index) {
    return pAgent->IsOutputLinkChangeAdd(index);
}

int getNumberCommands(sml::Agent* pAgent) {
    return pAgent->GetNumberCommands();
}

bool commands(sml::Agent* pAgent) {
    return pAgent->Commands();
}

sml::Identifier* getCommand(sml::Agent* pAgent, int index) {
    return pAgent->GetCommand(index);
}

void commit(sml::Agent* pAgent) {
    pAgent->Commit();
}

bool isCommitRequired(sml::Agent* pAgent) {
    return pAgent->IsCommitRequired();
}

void setAutoCommit(sml::Kernel* pKernel, bool state) {
    pKernel->SetAutoCommit(state);
}

//##################### Identifier ######################
const char* getCommandName(sml::Identifier* identifier) {
    return identifier->GetCommandName();
}

const char* getParameterValue(sml::Identifier* identifier, const char* attribute) {
    return identifier->GetParameterValue(attribute);
}

void addStatusComplete(sml::Identifier* identifier) {
    identifier->AddStatusComplete();
}

void addStatusError(sml::Identifier* identifier) {
    return identifier->AddStatusError();
}

#pragma endregion

//##################### Run Agent ######################
#pragma region Run
char const* runSelf(sml::Agent* pAgent, int numberSteps, sml::smlRunStepSize stepSize = sml_DECIDE) {
    return pAgent->RunSelf(numberSteps, stepSize);
}

void runSelfForever(sml::Agent* pAgent) {
    pAgent->RunSelfForever();
    //cout << "Agent Running" << endl;
}

void runSelfTilOutput(sml::Agent* pAgent) {
    pAgent->RunSelfTilOutput();
}

#pragma endregion

//##################### Events ######################
#pragma region Events
// TODO: only create register events functions here and use the C# sml example to write the calls inside Unity

//###### Print
int registerForPrintEvent(sml::Agent* pAgent, sml::smlPrintEventId id, sml::PrintEventHandler handler, void* pUserData, bool ignoreOwnEchos = true, bool addToBack = true) {
    return pAgent->RegisterForPrintEvent(id, handler, pUserData, ignoreOwnEchos, addToBack);
}

bool unregisterForPrintEvent(sml::Agent* pAgent, int callbackId) {
    return pAgent->UnregisterForPrintEvent(callbackId);
}

//###### Update
int registerForUpdateEvent(sml::Kernel* pKernel, sml::smlUpdateEventId id, sml::UpdateEventHandler handler, void* pUserData, bool addToBack = true) {
    return pKernel->RegisterForUpdateEvent(id, handler, pUserData, addToBack);
}

bool unregisterForUpdateEvent(sml::Kernel* pKernel, int callbackId) {
    return pKernel->UnregisterForUpdateEvent(callbackId);
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
Identifier* inputId;

void runEventHandler(smlRunEventId id, void* pUserData, Agent* pAgent, smlPhase phase) {
    char cmd[20];
    strcpy_s(cmd, "print --depth 3 ");
    strcat_s(cmd, inputId->GetIdentifierName());

    //cout << cmd << endl;
    const char* answer = pAgent->ExecuteCommandLine(cmd);
    //cout << answer << endl;
}

//int main(int argc, char* argv[]) {
//    sml::Kernel* pKernel = createKernelInNewThread();
//    sml::Agent* pAgent = createAgent("teste", pKernel);
//
//    //NOT WORKING:
//    //spawnDebugger(pAgent, -1, "C:\\Users\\felip\\Desktop\\SOAR\\SoarTutorial_9.6.1-Multiplatform\\bin\\SoarJavaDebugger.jar");
//    
//    //NOT WORKING:
//    //registerForProductionAddedEvent(pAgent);
//
//    //registerForPrintEvent(pAgent);
//
//    inputId = getInputLink(pAgent);
//    Identifier* squareId = createIdWME(pAgent, inputId, "square");
//    Identifier* positionId = createIdWME(pAgent, squareId, "position");
//    FloatElement* xId = createFloatWME(pAgent, positionId, "x", 1.5);
//    FloatElement* yId = createFloatWME(pAgent, positionId, "y", 1.5);
//    commit(pAgent);
//
//    cout<< "SQUARE IDENTIFIER GET NAME ";
//    cout<<getCommandName(squareId)<<endl;
//
//    loadProductions(pAgent, "C:\\Users\\felip\\Documents\\GitHub\\making-soar-work\\UnitySoarSquareEx\\DLL\\SoarUnityAPI\\x64\\Release\\initialize-square-agent.soar", true);
//    loadProductions(pAgent, "C:\\Users\\felip\\Documents\\GitHub\\making-soar-work\\UnitySoarSquareEx\\DLL\\SoarUnityAPI\\x64\\Release\\move-north.soar", true);
//    
//    //std::string trace = "RUN EVENT: ";
//    //pAgent->RegisterForRunEvent(smlEVENT_BEFORE_DECISION_CYCLE, runEventHandler, &trace);
//
//    runSelfForever(pAgent);
//
//    std::cin.ignore();
//}
#pragma endregion
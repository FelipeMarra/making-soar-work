// SoarCppConsoleApp.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "SoarCppConsoleApp.h"
#include "include/sml_Client.h"

using namespace sml;
using namespace std;

int soar_test() {
    int debug = 0;
    // Create an instance of the Soar kernel in our process
    Kernel* pKernel = Kernel::CreateKernelInNewThread();
    debug++; //1

    // Check that nothing went wrong.  We will always get back a kernel object
    // even if something went wrong and we have to abort.
    if (pKernel->HadError())
    {
        cout << pKernel->GetLastErrorDescription() << endl;
        return -1;
    }

    // Create a Soar agent named "test"
    // NOTE: We don't delete the agent pointer.  It's owned by the kernel
    sml::Agent* pAgent = pKernel->CreateAgent("test");
    debug++; //2

    // Check that nothing went wrong
    // NOTE: No agent gets created if there's a problem, so we have to check for
    // errors through the kernel object.
    if (pKernel->HadError())
    {
        cout << pKernel->GetLastErrorDescription() << endl;
        return -2;
    }

    // Load some productions
    pAgent->LoadProductions("testsml.soar");
    debug++; //3

    if (pAgent->HadError())
    {
        cout << pAgent->GetLastErrorDescription() << endl;
        return -3;
    }

    Identifier* pInputLink = pAgent->GetInputLink();
    debug++; //4

    // Create (I3 ^plane P1) (P1 ^type Boeing747 ^speed 200 ^direction 50.5) on
    // the input link.  (We don't own any of the returned objects).
    Identifier* pID = pAgent->CreateIdWME(pInputLink, "plane");
    StringElement* pWME1 = pAgent->CreateStringWME(pID, "type", "Boeing747");
    IntElement* pWME2 = pAgent->CreateIntWME(pID, "speed", 200);
    FloatElement* pWME3 = pAgent->CreateFloatWME(pID, "direction", 50.5);
    debug++; //5

    // Send the changes to working memory to Soar
    // With 8.6.2 this call is optional as changes are sent automatically.
    pAgent->Commit();
    debug++; //6

    // Run Soar for 2 decisions
    pAgent->RunSelf(2);
    debug++; //7

    // Change (P1 ^speed) to 300 and send that change to Soar
    pAgent->Update(pWME2, 300);
    debug++; //8

    pAgent->Commit();
    debug++; //9

    // Run Soar until it generates output or 15 decision cycles have passed
    // (More normal case is to just run for a decision rather than until output).
    pAgent->RunSelfTilOutput();
    debug++; //10

    // Go through all the commands we've received (if any) since we last ran Soar.
    int numberCommands = pAgent->GetNumberCommands();
    debug++; //11

    for (int i = 0; i < numberCommands; i++)
    {
        Identifier* pCommand = pAgent->GetCommand(i);

        std::string name = pCommand->GetCommandName();
        std::string speed = pCommand->GetParameterValue("speed");

        // Update environment here to reflect agent's command

        // Then mark the command as completed
        pCommand->AddStatusComplete();

        // Or could do the same manually like this:
                // pAgent->CreateStringWME(pCommand, "status", "complete") ;
    }
    debug++; //12

    // See if anyone (e.g. a debugger) has sent commands to Soar
    // Without calling this method periodically, remote connections will be ignored if
    // we choose the "CreateKernelInCurrentThread" method.
    pKernel->CheckForIncomingCommands();
    debug++; //13

    // Create an example Soar command line
    std::string cmd = "excise --all";
    debug++; //14

    // Execute the command
    char const* pResult = pKernel->ExecuteCommandLine(cmd.c_str(), pAgent->GetAgentName());
    debug++; //15

    // Shutdown and clean up
    pKernel->Shutdown();   // Deletes all agents (unless using a remote connection)
    delete pKernel;                // Deletes the kernel itself
    debug++; //16

    return debug;
}
# Making Soar Work w/ Unity
 > making the <a href="https://soar.eecs.umich.edu/"> Soar <a> cognitive architecture (version  9.6.1) work with the <a href="https://unity.com/"> Unity <a> game engine (version 2021.3.14.f1). Currently the integration is only for windows, but Soar and Unity are multiplatform.

## Summary
Creating a C++ DLL to expose your own created functions that make use of Soar, or expose Soar's functions in a more direct way, will work if done correctly. Besides the <a href="https://www.mono-project.com/docs/advanced/pinvoke/#invoking-unmanaged-code">  Mono docs <a> recommendation for generating the code automatically with swig, using the C# sml Dlls that come with Soar (which are generated with swig) will crash Unity if you register for a Soar event. <br>

In the example provided in this repo the second approach was chosen - expose Soar functions more directly - to recreate the Soar classes inside Unity. This approach will minimize the time one passes creating the DLL, and facilitate troubleshooting. </br>

The current example provides functions from the Kernel, Agent and WMEs classes. The <a href="https://github.com/FelipeMarra/making-soar-work/tree/main/UnitySoarSquareEx/DLL/SoarUnityAPI/x64/Release"> SoarUnity.dll <a> exports the functions from the Soar.dll - also, it can print from its C++ code into Unity's console -, and inside the Unity project the classes are created based on those to allow the use of Soar. Although they're not complete yet - the Agent one is in a more advanced stage - the present classes are suficient to create agentes. All of the imported functions are documented with the text from the original docs plus some tips to deal with pointers in C#. Some helper classes were also created to process the commands received from Soar. </br>

The following teaches how to get Soar working with Unity and presents a achitecture to receive and process its commands. </br>

# Exporting the Soar DLL
> This example was created with Visual Studio and only thought to work on windows for now. The DLL project can be found <a href="https://github.com/FelipeMarra/making-soar-work/tree/main/UnitySoarSquareEx/DLL/SoarUnityAPI"> here <a>.

#### 1. Create a new project in Visual Studio as a Dynamic-Link Library (DLL). </br>

#### 2. Import the Soar Library inside the project. </br>
To do that go to `Project > Properties > C/C++ > General > Additional Include Directories` and add the <a href="https://github.com/FelipeMarra/making-soar-work/tree/main/UnitySoarSquareEx/DLL/SoarSuite_9.6.1"> Soar Suit <a> path. Next head towards `Project > Properties > Linker > Additional Library Directories` to add a reference to the <a href="https://github.com/FelipeMarra/making-soar-work/tree/main/UnitySoarSquareEx/DLL/SoarSuite_9.6.1/bin/win_x86-64"> Soar.lib <a> path. Finally, inside `Linker > Input > Additional Dependencies` add `Soar.lib;`. Now inlcude Soar's header:

``` C++
#include "include/sml_Client.h"
```

#### 3. Create DLL header and .cpp
In the right lateral menu right click over the Header Files folder. Select `Add > New Item > Header File` and create a .h with your project's name. Now on the Source Files folder do the same process to create a .cpp with the project's name.

#### 4. Cpp code
Export the Soar functions by receiving the classes pointers as parameters and returning their functions.

``` C++
sml::Agent* createAgent(const char* name, sml::Kernel* pKernel) {
    sml::Agent* pAgent = pKernel->CreateAgent(name);

    if (pKernel->HadError()) {
        printError(pKernel->GetLastErrorDescription(), "createSoarAgent: ");
        return NULL;
    }

    return pAgent;
}
```
#### 5. Header code
To create the header file, just paste the functions signatures preceded by `extern "C"` and `__declspec(dllexport)`.

``` C++
#pragma once 

#ifdef SOARUNITYAPI_EXPORTS
#define SOARUNITYAPI_API __declspec(dllexport)
#else
#define SOARUNITYAPI_API __declspec(dllimport)
#endif

extern "C" {
    SOARUNITYAPI_API sml::Agent* createAgent(const char*, sml::Kernel*);
}
```
#### 6. Build DLL & Post Build Command
Go to `Project > Properties > Build Events > Post Build Event > Command line` and - for this example file structure - use the code 
```
xcopy /y /d "$(OutputPath)SoarUnityAPI.dll" "$(ProjectPath)..\..\..\..\..\UnitySoarSquareEx\Assets\Scripts\AI\Soar\DLL"
```
To copy the built DLL inside the Unity project. To build the DLL just go `Build > Build Solution`.

>  Notice that inside Unity both Soar's and your's DLLs must be in the same folder
</br>

# Importing & Using the DLL Inside Unity
## 1. Import the functions
Inside a C# script on Unity use the attribute [DllImport("YOUR_DLL_NAME")] and the keywords `static extern` before every function signature one wants to import.
``` C#
[DllImport("SoarUnityAPI")]
private static extern IntPtr createAgent(string name, IntPtr pKernel);
 ```
The IntPtr class allows you to receive pointers to C++ classes, strings, etc. <a href="https://www.mono-project.com/docs/advanced/pinvoke/"> Mono docs <a> will explain
in detail how to import your functions, but here you go some tips about that and using Soar functions in general:

#### => Load productions
For me only worked passing the full path. Use Application.dataPath + "PATH_FROM_ASSETS".

#### => Send pointer of C# object to C++
``` C#
GCHandle data = GCHandle.Alloc(YOUR_OBJECT);
IntPtr dataPtr = GCHandle.ToIntPtr(data);
```
A pointer allocated in that way can then be typecasted like:
  
``` C#
YOUR_OBJECT myObj = (YOUR_OBJECT_TYPE)((GCHandle)dataPtr).Target;
```

And to free it after use: 
``` C#
data.Free()
```

#### => Receive strings from C++ 
Always receive your strings as IntPtr. Receiving as a string will cause the C# Garbage Collector to deallocate it. To convert your IntPtr to string inside Unity use:
  
``` C#
string message = Marshal.PtrToStringAnsi(pMessage);
```
 
## 2. Reconstruct the classes
Import the functions inside a C# class that stores the pointer for the Agent, for example. Then use that pointer to create public functions for the class that don't
require the user to pass the agent's pointer as a parameter
``` C#
// Import as private from DLL
[DllImport("SoarUnityAPI")]
private static extern int loadProductions(IntPtr pAgent, string path, bool echoResults);

// Create public version that uses the class' cached pointer to the agent
public int LoadProductions(string path, bool echoResults = true) {
    return loadProductions(_pAgent, path, echoResults);
}
```

## Printing from C++ functions into the Unity console 
This example made use of <a href="https://stackoverflow.com/questions/43732825/use-debug-log-from-c"> this <a> StackOverflow answer to be able to print from the C++ code into the Debug Console.

 </br>

# Square Agent
The agent is a simple square. The square can move in one of four directions from the cardinal points. Once one direction is chosen the agente will prefere to maintain it, only changing when approaching the defined borders. 

![Video_1676461153_AdobeExpress](https://user-images.githubusercontent.com/89817439/219099315-89599ddd-f75d-47fc-8597-c0694d0310e9.gif)

## Running Agent
In the current version of the example the agent is running in the Update function using RunSelfTilOutput. In other words, once per frame - when it isn't blocked - it will have its input updated, make a decision and have its output processed. It might be interesting to run the agent inside a <a href="https://docs.unity3d.com/Manual/JobSystem.html"> Job <a>, because despite the fact that Soar's kernel runs in an independent thread, the program will be blocked to wait its decision cycle.
```C#
void Update() {
    if(!agentIsLocked) {
        UpdateSoarInputData();
        _agent.RunSelfTilOutput();
    }
}
```
## Reacting to Events
Since all functions inside the callbacks need to be static, it makes sense to use Unity's events so nonstatic functions can be called. This approach also improved the agent initialization performance if compared to call the functions executed by the events directly into the UpdateEventCallback. 

When the UpdateEventCallback (snippet below) function is called by Soar the agent is locked, a command list is created and the event to process the commands is called.
```C#
SquareAgent.Instance.LockAgent();

int numCmds = _agent.GetNumberCommands();

List<SoarCmd> cmds = new List<SoarCmd>();

for (int i = 0; i < numCmds; i++) {
    Identifier cmdId = _agent.GetCommand(i);
    SoarCmd cmd = null;

    switch (SoarCmd.GetTypeFromIdentifier(cmdId)) {
        case SoarCmdType.move:
            cmd = new SoarMoveCmd(cmdId);
            break;
        default:
            Debug.Log("<color=red>################ UNKNOWN COMMAND " + cmdId.GetCommandName() + "</color>");
            break;
    }

    cmds.Add(cmd);

    if(cmds.Count == 0) {
        SquareAgent.Instance.UnlockAgent();
        return;
    }

    EventHandler.CallCommandEvent(cmds);
}
```

Each SoarCmd class encapsulates its Run and Reset logics, also containing a priority number that is used by the AgentCmdManager class (snippet below) to reorder the commands accordingly before start executing then. After a command is executed the AddCompleteAndResetCommand method of the AgentCmdManager must be called to the add status complete augmentation to the commands and process the next one in the stack. After all the commands are executed, the agent will be unlocked, allowing it to receive the update, make a decision, and get it's output processed again.

```C#
public void SortAndRunCmds(List<SoarCmd> cmds) {
    _cmds = cmds;
    _cmds.Sort((e1,e2) => {
        if(e1.priority > e2.priority){
            return 1;
        }
        if(e1.priority < e2.priority){
            return -1;
        }
        return 0;
    });
    RunFirstCmd();
}

public void RunFirstCmd() {
    _currentCmd = _cmds[0];
    _currentCmd.Run();
}

public void AddCompleteAndResetCommand() {
    if(_currentCmd.type != SoarCmdType.none) {
        _currentCmd.AddStatusComplete();
        ResetCommand();
    }
}

private void ResetCommand() {
    _currentCmd.Reset();
    _currentCmd = new SoarCmd();

    _cmds.RemoveAt(0);
    if(_cmds.Count > 0) {
        RunFirstCmd();
    } else {
        SquareAgent.Instance.UnlockAgent();
    }
}
```

## Finalizing the Agent
In the RegisterForEvents method inside the SquareAgent class, one can observe that they are being added to a list that is used for the SoarUtils.UnregisterForEvents, inside OnDisable, to unregister from the events. This approach made the code much cleaner than storing every event data pointer and id on the top of the class and unsubscribing from them one by one in the OnDisable function.

``` C#
void OnDisable(){
    SoarUtils.UnregisterForEvents(events, _agent, _kernel);
    _kernel.Shutdown();
}
```

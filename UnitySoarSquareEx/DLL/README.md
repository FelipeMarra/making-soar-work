# Exporting the Soar DLL
> This example was created with Visual Studio and only thought to work on windows for now. The DLL project can be found <a href="https://github.com/FelipeMarra/making-soar-work/tree/main/UnitySoarSquareEx/DLL/SoarUnityAPI"> here <a>.

### 1. Create a new project in Visual Studio as a Dynamic-Link Library (DLL). </br>

### 2. Import the Soar Library inside the project. </br>
To do that go to `Project > Properties > C/C++ > General > Additional Include Directories` and add the <a href="https://github.com/FelipeMarra/making-soar-work/tree/main/UnitySoarSquareEx/DLL/SoarSuite_9.6.1"> Soar Suit <a> path. Next head towars `Project > Properties > Linker > Additional Library Directories` to add a reference to the <a href="https://github.com/FelipeMarra/making-soar-work/tree/main/UnitySoarSquareEx/DLL/SoarSuite_9.6.1/bin/win_x86-64"> Soar.lib <a> path. Finally, inside `Linker > Input > Additional Dependencies` add `Soar.lib;`. Now to use Soar just use 

``` C++
#include "include/sml_Client.h"
```

### 3. Creathe DLL header and .cpp
In the right lateral menu right click over the Header Files folder. Select `Add > New Item > Header File` and create a .h with you project's name. Now on the Source Files folder do the same process to create a .cpp with the projectc name.

### 4. Cpp code
Export the Soar functions by receiving the classes pointers as parameters and returning they're functions.

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
### 5. Header code
Paste the functions signatures preceded by `extern "C"` and `__declspec(dllexport)`.

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
### 6. Build DLL & Post Build Command
Go to `Project > Properties > Build Events > Post Build Event > Command line` and - for this example file structure - use the code 
```
xcopy /y /d "$(OutputPath)SoarUnityAPI.dll" "$(ProjectPath)..\..\..\..\..\UnitySoarSquareEx\Assets\Soar\DLL"
```
To copy the built DLL inside the Unity project. To build the DLL just go `Build > Build Solution`.

>  Notice that inside Unity both Soar's and your's DLLs must be in the same folder

# Importing & Using the DLL Inside Unity
 ### 1. Import the functions
 Inside a C# script on Unity use the attribute [DllImport("YOUR_DLL_NAME")] and the keywords `static extern` beforte every function signature one wants to import.
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
IntPtr dataPtr = GCHandle.ToIntPtr(userData);
```
A pointer allocated in that way can than be type casted like:
  
``` C#
YOUR_OBJECT data = (YOUR_OBJECT_TYPE)((GCHandle)userDataPtr).Target;
// And to free it afeter use: 
data.Free()
```
#### => Receive string from C++ 
Ways receive your strings as IntPtr. Receiving as string will cause the C# Garbage Collector to dealocate it. To convert your IntPtr to string inside Unity use
  
``` C#
string message = Marshal.PtrToStringAnsi(pMessage);
```
 
### 2. Reconstruct the classes
Import the functions inside a C# class that stores the pointer for the Agent for example. Then use that pointer to create public functions for the class that don't
require the user to pass the agent's pointer as a parameter
``` C#
// Import as private from DLL
[DllImport("SoarUnityAPI")]
private static extern int loadProductions(IntPtr pAgent, string path, bool echoResults);

// Create public version that uses the class cashes pointer to the agent
public int LoadProductions(string path, bool echoResults = true) {
    return loadProductions(_pAgent, path, echoResults);
}
```
 

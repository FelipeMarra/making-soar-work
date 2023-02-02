# Making Soar Work w/ Unity
 > making the SOAR 9.6.1 cognitive architecture work with Unity 2021.3.14.f1

## Summary
Basically creating a DLL to interact with Soar directly using C++ works. Notice that inside Unity both Soar and your DLLs must be in the same folder. <br> 

Using the C# sml directly inside unity, or via a C# DLL will crash unity if you register for a Soar event (at least in the way I did it, maybe I've done it wrong).

## Creating a DLL to use Soar & importing into Unity

> In the current example there is a SoarUnityAPI.dll that creates a kernel and an agent. Also, it can pint from it's C++ code into Unity's console. The projects can already be used as a simple template.

better tutorial wll be provided soon.

# ATENTION!!!: THE FOLLOWING IS A WORK IN PROGRESS. THIS DOC FOR NOW IS ONLY A PLANNING. NOT IMPLEMENTED YET!!!
# /UnitySoarSquareEx
> The following will explain the UnitySoarSquareEx.
## The agent
The agent will be a simple square. The square can rotate and move in one of the for directions. Rotation and move will be allowd to execute in parallel, but the square will be locked to move in only one direction at a time. When aproaching the screen berders the square will be blocked to continue in that direction. 

## Integration Architecture
### Unity
This example runs Soar inside a <a href="https://docs.unity3d.com/Manual/JobSystem.html">Job</a>. The Job System uses multithread to enhance performance. Besides the fact that Soar is fast this examples runs one Soar Step per frame, so it makes sence to let this logic in another thread. 

The high level scheme looks something like:

```
// Job
Update:
 Input Information from the InputStack Into Soar
 Run Soar Step
 Get Output
 Call Action Events
```

```
// Rotate/Move Event Handler
Rotate/Move X degrees/units per second for T seconds 
 then Add status complete to this action inside the InputStack
```

![UnityXSoar_SM](https://user-images.githubusercontent.com/89817439/215845072-817ad955-adbd-4ee4-b046-d3f63c1fc878.png)


The actions will be sent through events. The events will use the `<action> ^status complete` convention - that are used in Soar's Eaters and Tanks examples - to inform the agent that the action was completed.

### Soar 
```
# io top state: 
^io
  ^input-link
    ^blocked << top left bottom right >>
  ^output-link
    ^rotate
      ^degrees-per-second 10-180
      ^seconds 1-5
      ^status completed (or this WME is non existent)
    ^move
      ^units 10-500
      ^seconds 1-5
      ^status completed (or this WME is non existent)
 ```

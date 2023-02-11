# Making Soar Work w/ Unity
 > making the SOAR 9.6.1 cognitive architecture work with Unity 2021.3.14.f1

## Summary
Basically, creating a DLL to interact directly with Soar using C++ works. Notice that inside Unity both Soar and your DLLs must be in the same folder. <br> 

Using the C# sml directly inside unity, or via a C# DLL will crash Unity if you register for a Soar event (at least in the way I did it, maybe I've done it wrong).

## Current Status
The current example provides some functions from the Kernel, Agent, and Identifier classes. The SoarUnity.dll exports the functions from the Soar.dll - also, it can print from its C++ code into Unity's console -, and inside the Unity project the classes are created based on those to allow the use of Soar. They're not complete yet - the Agent one is in a more advanced stage. All of the imported functions are documented with the original docs plus some tips to deal with pointers in C#. </br>

At this point the project is an implementation of the `move-north` operator from the `eaters` tutorial - it makes a square move north. The code can already be used as a simple template for creating you're won agents in Unity, but a better example will be provided soon, accompanied by a series of tutorials with everything needed to run Soar inside Unity.

# BELLOW THIS POINT THIS DOC ONLY A PLANNING.
# /UnitySoarSquareEx
> The following will explain the UnitySoarSquareEx.
## The agent
The agent will be a simple square. The square can rotate and move in one of the for directions. Rotation and move will be allowd to execute in parallel, but the square will be locked to move in only one direction at a time. When aproaching the screen berders the square will be blocked to continue in that direction. 

## Integration Architecture
### Unity

### Soar 
```
# io top state: 
^io
  ^input-link
    ^blocked << top left bottom right >>
  ^output-link
    ^rotate
      ^direciton << left right >>
      ^status completed (or this WME is non existent)
    ^move
      ^direciton << top left bottom right >>
      ^status completed (or this WME is non existent)
 ```

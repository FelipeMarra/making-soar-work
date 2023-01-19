# making Soar work with Unity
 making the SOAR 9.6.1 cogninitve architecture work with Unity 2021.3.14.f1

Basically creating a DLL to interact with Soar directly using C++ works. Notice that inside Unity both Soar and your DLLs must be in the same folder. A decent tutorial will be provided here as soon as I have a better understanding about using Soar and making it intaract with Unity. <br> 

Using the C# sml directly inside unity, or via a C# DLL will crash unity if you register for a Soar event (at least in the way i did it, maybe i've donw it wrong). It probably have to do with a null pointer to the callback function or something like that. Also a better description of the error will be provided soon.

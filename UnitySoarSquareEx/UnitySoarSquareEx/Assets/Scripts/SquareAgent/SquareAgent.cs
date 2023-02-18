using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using smlUnity;

public class SquareAgent : SingletonMonobehavior<SquareAgent> {

    private Kernel _kernel;
    private static Agent _agent;

    private static StringElement[] blockedIds = new StringElement[4];

    private List<EventData> events = new List<EventData>();

    void Start() {
        _kernel = Kernel.CreateKernelInNewThread();

        _kernel.SetAutoCommit(false);

        _agent = _kernel.CreateAgent("square");

        CreateBaseInputWMEs();

        RegisterForEvents();

        _agent.LoadProductions(Application.dataPath + "/Soar/Productions/square-agent.soar");
    }

    void Update() {
        _agent.RunSelfTilOutput();
    }

    void OnDisable(){
        SoarUtils.UnregisterForEvents(events, _agent, _kernel);
        _kernel.Shutdown();
    }

    void CreateBaseInputWMEs() {
        Identifier inputId = _agent.GetInputLink();
        Identifier squareId = _agent.CreateIdWME(inputId, "square");
        Identifier blockedId = _agent.CreateIdWME(squareId, "blocked");
        blockedIds[0] = _agent.CreateStringWME(blockedId, "north", "no");
        blockedIds[1] = _agent.CreateStringWME(blockedId, "east", "no");
        blockedIds[2] = _agent.CreateStringWME(blockedId, "south", "no");
        blockedIds[3] = _agent.CreateStringWME(blockedId, "west", "no");
        _agent.Commit();
    }

//##################### Events ######################
#region Events
    void RegisterForEvents() {
        //Print
        GCHandle printUserData = GCHandle.Alloc("PRINT EVENT: ");
        int printId = _agent.RegisterForPrintEvent(smlPrintEventId.smlEVENT_PRINT, PrintEventCallback, GCHandle.ToIntPtr(printUserData));
        events.Add(new EventData(printUserData, printId, smlUnity.EventType.PRINT));

        //Update
        GCHandle eventUserData = GCHandle.Alloc("UPDATE EVENT: ");
        int updateId = _kernel.RegisterForUpdateEvent(smlUpdateEventId.smlEVENT_AFTER_ALL_OUTPUT_PHASES, UpdateEventCallback, GCHandle.ToIntPtr(eventUserData));
        events.Add(new EventData(eventUserData, updateId, smlUnity.EventType.UPDATE));
    }

    //Update
    static void PrintEventCallback(smlPrintEventId eventID, IntPtr pUserData, IntPtr pAgent, IntPtr pMessage) {
        string userData = (string)((GCHandle)pUserData).Target;
        string message = Marshal.PtrToStringAnsi(pMessage);
        Debug.Log(userData + message);
    }

    /// Update
    /// Since every function inside the update callback needs to be static it makes sense to use Unity's events so non static functions
    /// can be called. It also makes sense from a code organization perspective.
    static void UpdateEventCallback(smlUpdateEventId eventID, IntPtr pUserData, IntPtr pKernel, smlRunFlags runFlags) {
        List<Identifier> commands = new List<Identifier>();

        for (int i = 0; i < _agent.GetNumberCommands(); i++) {
            Identifier cmd = _agent.GetCommand(i);
            commands.Add(cmd);
        }

        EventHandler.CallCommandEvent(commands);

        EventHandler.CallUpdateBlockEvent();
    }

    public void UpdateBlock(string northValue, string eastValue, string southValue, string westValue) {
        _agent.Update(blockedIds[0], northValue);
        _agent.Update(blockedIds[1], eastValue);
        _agent.Update(blockedIds[2], southValue);
        _agent.Update(blockedIds[3], westValue);
        _agent.Commit();
    }

#endregion
}

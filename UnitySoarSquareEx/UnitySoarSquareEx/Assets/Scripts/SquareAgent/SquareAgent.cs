using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using smlUnity;

public class SquareAgent : SingletonMonobehavior<SquareAgent> {

    private Kernel _kernel;
    private static Agent _agent;

    ///<summary>Ids of the elements that represent a certain direction is blocked in north, east, south, west order</summary>
    private static StringElement[] blockIds = new StringElement[4];

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
        blockIds[0] = _agent.CreateStringWME(blockedId, "north", "no");
        blockIds[1] = _agent.CreateStringWME(blockedId, "east", "no");
        blockIds[2] = _agent.CreateStringWME(blockedId, "south", "no");
        blockIds[3] = _agent.CreateStringWME(blockedId, "west", "no");
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

    /// Print
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
        _agent.Update(blockIds[0], northValue);
        _agent.Update(blockIds[1], eastValue);
        _agent.Update(blockIds[2], southValue);
        _agent.Update(blockIds[3], westValue);
        _agent.Commit();
    }

#endregion
}

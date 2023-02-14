using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using smlUnity;

public class SquareAgent : SingletonMonobehavior<SquareAgent> {

    private Kernel _kernel;
    private static Agent _agent;

    private static IntPtr[] blockedIds = new IntPtr[4];

    private List<EventData> events = new List<EventData>();

    void Start() {
        _kernel = new Kernel();

        _kernel.SetAutoCommit(false);

        _agent = new Agent("square", _kernel);

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

    //Update
    static void UpdateEventCallback(smlUpdateEventId eventID, IntPtr pUserData, IntPtr pKernel, smlRunFlags runFlags) {
        List<Identifier> commands = new List<Identifier>();

        for (int i = 0; i < _agent.GetNumberCommands(); i++) {
            Identifier cmd = _agent.GetCommand(i);
            commands.Add(cmd);
        }

        EventHandler.CallCommandEvent(commands);

        EventHandler.CallUpdateBlockEvent(_agent, blockedIds[0], blockedIds[1], blockedIds[2], blockedIds[3]);
    }

#endregion
}

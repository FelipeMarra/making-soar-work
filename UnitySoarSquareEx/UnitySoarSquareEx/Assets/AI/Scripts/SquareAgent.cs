using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using smlUnity;

public class SquareAgent : MonoBehaviour {

    private Kernel _kernel;
    private Agent _agent;

    private List<EventData> events = new List<EventData>();

    void Start() {
        _kernel = new Kernel();

        _kernel.SetAutoCommit(false);

        _agent = new Agent("square", _kernel);

        RegisterForEvents();

        CreateBaseInputWMEs();

        _agent.LoadProductions(Application.dataPath + "/AI/SoarProductions/square-agent.soar");

        _agent.RunSelfForever();

        SoarUtils.UnregisterForEvents(events, _agent, _kernel);

        Debug.Log("<color='red'>SOAR STOPED</color>");
    }

//##################### Events ######################
#region Events
    static void PrintEventCallback(smlPrintEventId eventID, IntPtr pUserData, IntPtr pAgent, string message) {
        string userData = (string)((GCHandle)pUserData).Target;
        Debug.Log(userData + message);
    }

    static void UpdateEventCallback(smlUpdateEventId eventID, IntPtr pUserData, IntPtr pKernel, smlRunFlags runFlags) {
        string userData = (string)((GCHandle)pUserData).Target;
        Debug.Log(userData + " run flags " + runFlags);
    }

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

#endregion

    void CreateBaseInputWMEs(){
        IntPtr inputId = _agent.GetInputLink();
        IntPtr squareId = _agent.CreateIdWME(inputId, "square");
        IntPtr positionId = _agent.CreateIdWME(squareId, "position");
        IntPtr xId = _agent.CreateFloatWME(positionId, "x", transform.position.x);
        IntPtr yId = _agent.CreateFloatWME(positionId, "y", transform.position.y);
        _agent.Commit();
    }
}

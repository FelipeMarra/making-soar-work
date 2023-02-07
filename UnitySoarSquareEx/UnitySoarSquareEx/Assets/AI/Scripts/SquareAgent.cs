using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using static SoarManager;
using static SoarEvents;

public class SquareAgent : MonoBehaviour {

    private IntPtr _ptrKernel;
    private IntPtr _ptrAgent;

    private List<EventData> events = new List<EventData>();

    void Start() {
        InitAgent();

        RegisterForEvents();

        CreateBaseInputWMEs();

        loadProductions(_ptrAgent, Application.dataPath + "/AI/SoarProductions/square-agent.soar");

        runSelfForever(_ptrAgent);

        UnregisterForEvents(events, _ptrAgent, _ptrKernel);

        Debug.Log("<color='red'>SOAR STOPED</color>");
    }

    private void InitAgent() {
        _ptrKernel = createKernel();
        setAutoCommit(_ptrKernel, false);
        _ptrAgent = createAgent("square", _ptrKernel);
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
        int printId = registerForPrintEvent(_ptrAgent, smlPrintEventId.smlEVENT_PRINT, PrintEventCallback, GCHandle.ToIntPtr(printUserData));
        events.Add(new EventData(printUserData, printId, SoarEvents.EventType.PRINT));

        //Update
        GCHandle eventUserData = GCHandle.Alloc("UPDATE EVENT: ");
        int updateId = registerForUpdateEvent(_ptrKernel, smlUpdateEventId.smlEVENT_AFTER_ALL_OUTPUT_PHASES, UpdateEventCallback, GCHandle.ToIntPtr(eventUserData));
        events.Add(new EventData(eventUserData, updateId, SoarEvents.EventType.UPDATE));
    }

#endregion

    void CreateBaseInputWMEs(){
        IntPtr inputId = getInputLink(_ptrAgent);
        IntPtr squareId = createIdWME(_ptrAgent, inputId, "square");
        IntPtr positionId = createIdWME(_ptrAgent, squareId, "position");
        IntPtr xId = createFloatWME(_ptrAgent, positionId, "x", transform.position.x);
        IntPtr yId = createFloatWME(_ptrAgent, positionId, "y", transform.position.y);
        commit(_ptrAgent);
    }
}

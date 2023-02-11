using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using smlUnity;

public class PositionData{
    public static IntPtr pXId;
    public static IntPtr pYId;
    public static float x;
    public static float y;
    public static Vector3 incrementPosition;
    public static float speed = 2;
}

public class SquareAgent : MonoBehaviour {

    private Kernel _kernel;
    private static Agent _agent;
    public float speed;

    List<EventData> events = new List<EventData>();

    void Start() {
        PositionData.speed = speed;

        _kernel = new Kernel();

        _kernel.SetAutoCommit(false);

        _agent = new Agent("square", _kernel);

        CreateBaseInputWMEs();

        RegisterForEvents();

        _agent.LoadProductions(Application.dataPath + "/AI/SoarProductions/square-agent.soar");
    }

    void Update() {
        PositionData.x = this.transform.position.x;
        PositionData.y = this.transform.position.y;
        _agent.RunSelfTilOutput();
        this.transform.position += PositionData.incrementPosition;
    }

    void OnDestroy(){
        SoarUtils.UnregisterForEvents(events, _agent, _kernel);
        _kernel.Shutdown();
    }

    void CreateBaseInputWMEs() {
        Identifier inputId = _agent.GetInputLink();
        Identifier squareId = _agent.CreateIdWME(inputId, "square");
        Identifier positionId = _agent.CreateIdWME(squareId, "position");
        PositionData.pXId = _agent.CreateFloatWME(positionId, "x", this.transform.position.x);
        PositionData.pYId = _agent.CreateFloatWME(positionId, "y", this.transform.position.y);
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
        string userData = (string)((GCHandle)pUserData).Target;
        ExecuteCommands();
        UpdateInput();
        Debug.Log(userData + " (" + PositionData.x + "," + PositionData.y + ") ");
    }

    static void UpdateInput() {
        _agent.Update(PositionData.pXId, PositionData.x);
        _agent.Update(PositionData.pYId, PositionData.y);
        _agent.Commit();
    }

    static void ExecuteCommands() {
        Debug.Log("N OF CMDs " + _agent.GetNumberCommands());
        for (int i = 0; i < _agent.GetNumberCommands(); i++) {
            Identifier cmd = _agent.GetCommand(i);
            string cmdName = cmd.GetCommandName();
            Debug.Log("CMD NAME " + cmdName);

            if(cmdName == "move") {
                string directionName = cmd.GetParameterValue("direction");
                Vector3 directionVec = new Vector3();

                if(directionName == "north") {
                    directionVec = new Vector3(0f,1f,0f);
                }

                PositionData.incrementPosition = directionVec.normalized * PositionData.speed * Time.deltaTime;

                cmd.AddStatusComplete();
            }
        }
    }

#endregion
}

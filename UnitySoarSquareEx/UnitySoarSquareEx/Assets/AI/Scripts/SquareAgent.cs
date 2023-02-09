using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using smlUnity;

public class SquareAgent : MonoBehaviour {

    private Kernel _kernel;
    private Agent _agent;

    private IntPtr _pXId;
    private IntPtr _pYId;

    public float speed;

    private List<EventData> events = new List<EventData>();

    void Start() {
        _kernel = new Kernel();

        _kernel.SetAutoCommit(false);

        _agent = new Agent("square", _kernel);

        CreateBaseInputWMEs();

        RegisterForEvents();

        _agent.LoadProductions(Application.dataPath + "/AI/SoarProductions/square-agent.soar");
    }

    void Update() {
        _agent.RunSelfTilOutput();
    }

    void OnDestroy(){
        SoarUtils.UnregisterForEvents(events, _agent, _kernel);
        //TODO _kernel.Shutdown();
    }

    void CreateBaseInputWMEs() {
        Identifier inputId = _agent.GetInputLink();
        Identifier squareId = _agent.CreateIdWME(inputId, "square");
        Identifier positionId = _agent.CreateIdWME(squareId, "position");
        _pXId = _agent.CreateFloatWME(positionId, "x", this.transform.position.x);
        _pYId = _agent.CreateFloatWME(positionId, "y", this.transform.position.y);
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
    static void PrintEventCallback(smlPrintEventId eventID, IntPtr pUserData, IntPtr pAgent, string message) {
        string userData = (string)((GCHandle)pUserData).Target;
        Debug.Log(userData + message);
    }
    
    //Update
    void UpdateEventCallback(smlUpdateEventId eventID, IntPtr pUserData, IntPtr pKernel, smlRunFlags runFlags) {
        string userData = (string)((GCHandle)pUserData).Target;
        //Debug.Log(userData + " run flags " + runFlags); TODO runFlags crashing
        ExecuteCommands();
        UpdateInput();
        Debug.Log(userData + transform.position);
    }

    void UpdateInput() {
        _agent.Update(_pXId, this.transform.position.x);
        _agent.Update(_pYId, this.transform.position.y);
        _agent.Commit();
    }

    void ExecuteCommands() {
        Debug.Log("N OF CMDs " + _agent.GetNumberCommands());
        for (int i = 0; i < _agent.GetNumberCommands(); i++) {
            Identifier cmd = _agent.GetCommand(i);
            string cmdName = cmd.GetCommandName();
            Debug.Log("CMD NAME " + cmdName);
            if(cmdName == "move") {
                //North
                Vector3 direction = new Vector3(0f,1f,0f);

                ///TODO cmd.GetParameterValue

                this.transform.position += direction.normalized * speed * Time.deltaTime;

                cmd.AddStatusComplete();
            }
        }
    }

#endregion
}

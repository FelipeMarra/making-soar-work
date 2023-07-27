using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using smlUnity;

[RequireComponent(typeof(DebugCpp))]
public class SquareAgent : SingletonMonobehavior<SquareAgent> {

    private Kernel _kernel;
    private static Agent _agent;

    private bool agentIsLocked = false;

    ///<summary>Ids of the elements that represent a certain direction is blocked in north, east, south, west order</summary>
    private static StringElement[] blockIds = new StringElement[4];
    public string[] blockValues = new string[4];

    private List<EventData> events = new List<EventData>();

    void Start() {
        _kernel = Kernel.CreateKernelInNewThread();

        _kernel.SetAutoCommit(false);

        _agent = _kernel.CreateAgent("square");

        blockValues[0] = "no";
        blockValues[1] = "no";
        blockValues[2] = "no";
        blockValues[3] = "no";

        CreateBaseInputWMEs();

        RegisterForEvents();

        _agent.LoadProductions(Application.dataPath + "/Scripts/AI/Soar/Productions/square-agent.soar");
    }

    void Update() {
        if(!agentIsLocked) {
            UpdateSoarInputData();
            _agent.RunSelfTilOutput();
        }
    }

    public void LockAgent() {
        Debug.Log("<color=lightblue> LOCKED AGENT </color>");
        agentIsLocked = true;
    }

    public void UnlockAgent() {
        Debug.Log("<color=lightblue> UNLOCKED AGENT </color>");
        agentIsLocked = false;
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
        SquareAgent.Instance.LockAgent();

        int numCmds = _agent.GetNumberCommands();

        List<SoarCmd> cmds = new List<SoarCmd>();

        for (int i = 0; i < numCmds; i++) {
            Identifier cmdId = _agent.GetCommand(i);
            SoarCmd cmd = null;

            switch (SoarCmd.GetTypeFromIdentifier(cmdId)) {
                case SoarCmdType.move:
                    cmd = new SoarMoveCmd(cmdId);
                    break;
                default:
                    Debug.Log("<color=red>################ UNKNOWN COMMAND " + cmdId.GetCommandName() + "</color>");
                    break;
            }

            if(cmd != null && cmd.isStatusComplete) {
                //#TODO: Try to solve again wiht a soar rule that removes all status complete or in this point use a function to
                // remove the WME at once. This wont occur in this simple demo.
                Debug.Log("<color=red> @@@@@@@@@@@@@@@@@@@@@@ SKIPING COMPLETED " + cmd.name +" CMD @@@@@@@@@@@@@@@@ </color>");
            } else {
                cmds.Add(cmd);
            }
        }

        if(cmds.Count == 0) {
            Debug.Log("<color=red>################ NO COMMANDS RECEIVED ################</color>");
            SquareAgent.Instance.UnlockAgent();
            return;
        }

        Debug.Log("<color=lightblue> ********************** RECEIVED " + numCmds +" CMDS ********************** </color>");
        EventHandler.CallCommandEvent(cmds);
    }

    ///Updates the input data inside the Soar agent
    public void UpdateSoarInputData() {
        Debug.Log("<color=orange> UPDATED INPUT </color>");
        _agent.Update(blockIds[0], blockValues[0]);
        _agent.Update(blockIds[1], blockValues[1]);
        _agent.Update(blockIds[2], blockValues[2]);
        _agent.Update(blockIds[3], blockValues[3]);
        _agent.Commit();
    }
#endregion
}

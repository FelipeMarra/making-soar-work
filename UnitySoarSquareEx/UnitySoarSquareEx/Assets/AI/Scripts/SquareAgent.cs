using System;
using UnityEngine;
using System.Runtime.InteropServices;
using static SoarManager;
using static SoarEvents;

public class SquareAgent : MonoBehaviour {

    private IntPtr _ptrKernel;
    private IntPtr _ptrAgent;

    void Start() {
        Init();

        GCHandle userData = GCHandle.Alloc("PRINT EVENT: ");
        registerForPrintEvent(_ptrAgent, smlPrintEventId.smlEVENT_PRINT, PrintEventCallback, GCHandle.ToIntPtr(userData));

        CreateBaseInputWMEs();
        loadProductions(_ptrAgent, Application.dataPath + "/AI/SoarProductions/square-agent.soar");
        runSelfForever(_ptrAgent);

        userData.Free();

        Debug.Log("<color='red'>SOAR STOPED</color>");
    }

    private void Init() {
        _ptrKernel = createKernel();
        setAutoCommit(_ptrKernel, false);
        _ptrAgent = createAgent("square", _ptrKernel);
    }

    
    static void PrintEventCallback(smlPrintEventId eventID, IntPtr userDataPtr, IntPtr agentPtr, string message) {
        string userData = (string)((GCHandle)userDataPtr).Target;
        Debug.Log(userData + message);
    }

    void CreateBaseInputWMEs(){
        IntPtr inputId = getInputLink(_ptrAgent);
        IntPtr squareId = createIdWME(_ptrAgent, inputId, "square");
        IntPtr positionId = createIdWME(_ptrAgent, squareId, "position");
        IntPtr xId = createFloatWME(_ptrAgent, positionId, "x", transform.position.x);
        IntPtr yId = createFloatWME(_ptrAgent, positionId, "y", transform.position.y);
        commit(_ptrAgent);
    }
}

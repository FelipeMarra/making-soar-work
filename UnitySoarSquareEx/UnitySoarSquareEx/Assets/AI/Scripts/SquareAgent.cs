using System;
using UnityEngine;

public class SquareAgent : MonoBehaviour {

    private IntPtr _ptrKernel;
    private IntPtr _ptrAgent;

    void Start() {
        Init();
        SoarManager.registerForPrintEvent(_ptrAgent);
        CreateBaseInputWMEs();
        SoarManager.loadProductions(_ptrAgent, Application.dataPath + "/AI/SoarProductions/square-agent.soar");
        SoarManager.runSelfForever(_ptrAgent);
        Debug.Log("<color='red'>SOAR STOPED</color>");
    }

    private void Init() {
        _ptrKernel = SoarManager.createKernel();
        SoarManager.setAutoCommit(_ptrKernel, false);
        _ptrAgent = SoarManager.createAgent("square", _ptrKernel);
    }

    void CreateBaseInputWMEs(){
        IntPtr inputId = SoarManager.getInputLink(_ptrAgent);
        IntPtr squareId = SoarManager.createIdWME(_ptrAgent, inputId, "square");
        IntPtr positionId = SoarManager.createIdWME(_ptrAgent, squareId, "position");
        IntPtr xId = SoarManager.createFloatWME(_ptrAgent, positionId, "x", transform.position.x);
        IntPtr yId = SoarManager.createFloatWME(_ptrAgent, positionId, "y", transform.position.y);
        SoarManager.commit(_ptrAgent);
    }

    //TODO: Debug stop working for print event when it is inside other function
    // private void RegisterForEvents(){
    //     SoarManager.registerForPrintEvent(_ptrAgent);
    // }
}

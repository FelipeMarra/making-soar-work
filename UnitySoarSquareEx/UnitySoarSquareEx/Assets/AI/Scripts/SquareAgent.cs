using System;
using UnityEngine;

public class SquareAgent : MonoBehaviour {

    private IntPtr _ptrKernel;
    private IntPtr _ptrAgent;

    void Start() {
        Init();
        SoarManager.registerForPrintEvent(_ptrAgent);
        SoarManager.runSelfForever(_ptrAgent);
        LoadProductions();
        //CreateBaseInputWMEs();
    }

    private void Init() {
        _ptrKernel = SoarManager.createKernel();
        _ptrAgent = SoarManager.createAgent("square", _ptrKernel);
    }

    //TODO: Debug stop working for print event when it is inside other function
    // private void RegisterForEvents(){
    //     SoarManager.registerForPrintEvent(_ptrAgent);
    // }

    private void LoadProductions(){
        SoarManager.loadProductions(_ptrAgent, Application.dataPath + "/AI/SoarProductions/square-agent/initialize-square-agent.soar");
        SoarManager.loadProductions(_ptrAgent, Application.dataPath + "/AI/SoarProductions/square-agent/move-north.soar");
    }

    void CreateBaseInputWMEs(){
        IntPtr inputId = SoarManager.getInputLink(_ptrAgent);
        IntPtr squareId = SoarManager.createIdWME(_ptrAgent, inputId, "square");
        IntPtr xId = SoarManager.createFloatWME(_ptrAgent, squareId, "x", transform.position.x);
        IntPtr yId = SoarManager.createFloatWME(_ptrAgent, squareId, "y", transform.position.y);
        SoarManager.commit(_ptrAgent);
    }
}

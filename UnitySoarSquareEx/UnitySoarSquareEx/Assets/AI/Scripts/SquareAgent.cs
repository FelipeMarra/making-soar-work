using System;
using UnityEngine;

public class SquareAgent : MonoBehaviour {

    private IntPtr _ptrKernel;
    private IntPtr _ptrAgent;

    void Start() {

        InitAgent();
        CreateBaseInputWMEs();
        SoarManager.registerForPrintEvent(_ptrAgent);
        SoarManager.registerForProductionAddedEvent(_ptrAgent);

        SoarManager.runSelfTilOutput(_ptrAgent);
    }

    private void InitAgent() {
        _ptrKernel = SoarManager.createKernel();
        _ptrAgent = SoarManager.createAgent("square", _ptrKernel);

        SoarManager.loadProductions(_ptrAgent, Application.dataPath + "/AI/SoarProductions/square-agent.soar");
    }

    void CreateBaseInputWMEs(){
        IntPtr inputId = SoarManager.getInputLink(_ptrAgent);
        IntPtr squareId = SoarManager.createIdWME(_ptrAgent, inputId, "square");
        IntPtr xId = SoarManager.createFloatWME(_ptrAgent, squareId, "x", transform.position.x);
        IntPtr yId = SoarManager.createFloatWME(_ptrAgent, squareId, "y", transform.position.y);
    }
}

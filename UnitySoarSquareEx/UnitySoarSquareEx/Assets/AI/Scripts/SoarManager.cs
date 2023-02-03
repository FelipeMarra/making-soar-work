using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class SoarManager : MonoBehaviour {
    [DllImport("SoarUnityAPI")]
    public static extern IntPtr createSoarKernel();

    [DllImport("SoarUnityAPI")]
    public static extern IntPtr createSoarAgent(string name, IntPtr kernel);

    [DllImport("SoarUnityAPI")]
    public static extern int loadSoarProductions(IntPtr agent, string path);

    IntPtr soarKernel;
    IntPtr squareAgent;

    void Start() {
        soarKernel = createSoarKernel();
        squareAgent = createSoarAgent("square", soarKernel);

        loadSoarProductions(squareAgent, Application.dataPath + "/AI/SoarProductions/square-agent.soar");
    }
}

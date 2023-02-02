using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class SoarManager : MonoBehaviour {
    [DllImport("SoarUnityAPI")]
    public static extern IntPtr createSoarKernel();

    //const char*
    [DllImport("SoarUnityAPI")]
    public static extern IntPtr createSoarAgent(string name, IntPtr kernel);

    IntPtr soarKernel;
    IntPtr squareAgent;

    void Start() {
        soarKernel = createSoarKernel();
        squareAgent = createSoarAgent("square", soarKernel);
    }
}

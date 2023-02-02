using System.Runtime.InteropServices;
using UnityEngine;

public class SoarManager : MonoBehaviour {
    [DllImport("SoarUnityAPI")]
    public static extern int createSoarKernel();

    //const char*
    [DllImport("SoarUnityAPI")]
    public static extern int createSoarAgent(string name);

    void Start() {
        createSoarKernel();
        createSoarAgent("square");
    }
}

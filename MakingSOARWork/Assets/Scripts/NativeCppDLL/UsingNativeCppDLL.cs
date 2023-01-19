using System.Runtime.InteropServices;
using UnityEngine;

public class UsingNativeCppDLL : MonoBehaviour
{
    [DllImport("SoarCppTest")]
    private static extern void soar_test();

    void Start() {
        soar_test();
    }
}

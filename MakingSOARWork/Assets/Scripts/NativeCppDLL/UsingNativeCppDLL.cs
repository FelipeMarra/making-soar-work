using System.Runtime.InteropServices;
using UnityEngine;

public class UsingNativeCppDLL : MonoBehaviour
{
    [DllImport("SoarCppTest")]
    public static extern void soar_test();

    void Start() {
        soar_test();
    }
}

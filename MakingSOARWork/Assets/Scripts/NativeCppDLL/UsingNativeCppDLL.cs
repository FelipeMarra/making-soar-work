using System.Runtime.InteropServices;
using UnityEngine;

public class UsingNativeCppDLL : MonoBehaviour
{
    [DllImport("SoarCppTest")]
    public static extern int soar_test();



    void Start()
    {
        Debug.Log(soar_test());
    }
}

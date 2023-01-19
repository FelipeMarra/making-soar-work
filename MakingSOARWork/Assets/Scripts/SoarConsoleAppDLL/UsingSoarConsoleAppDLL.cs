using System.Runtime.InteropServices;
using UnityEngine;

public class UsingSoarConsoleAppDLL : MonoBehaviour
{
    [DllImport("SoarCppConsoleApp")]
    public static extern int soar_test();



    void Start()
    {
        Debug.Log(soar_test());
    }
}

using System.Runtime.InteropServices;
using UnityEngine;

public class SoarManager : MonoBehaviour {
    [DllImport("SquareExDLL")]
    public static extern int soar_test();

    void Start() {
        Debug.Log(soar_test());
    }
}

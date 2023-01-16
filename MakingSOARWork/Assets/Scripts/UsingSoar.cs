using UnityEngine;
using TestCSharpSML;
using System;

public class UsingSoar : MonoBehaviour {
    static CSharpInterface test = new CSharpInterface();

    void Start() {
        RunSoar();
    }

    [STAThread]
    public static void RunSoar() {
        bool result = false;
        try {
            result = test.Test();
        }
        catch (Exception ex) {
            Debug.Log(ex);
        }

        Debug.Log("-----------------------------");
        if (result)
            Debug.Log("Tests assed");
        else
            Debug.Log("Tests failed");
    }
}

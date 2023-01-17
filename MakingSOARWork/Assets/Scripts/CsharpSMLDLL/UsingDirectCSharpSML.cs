using UnityEngine;
using TestCSharpSML;
using System;

/*
####################################################################
## Test Using the Soar's DLLs (inside plugins/soar) with a C# script
## calling them directly inside unity
WARNING: It crashes unity
#####################################################################
*/
public class UsingDirectCSharpSML : MonoBehaviour {
    static DirectCSharpSML test = new DirectCSharpSML();

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

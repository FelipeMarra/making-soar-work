using System;
using System.Runtime.InteropServices;

public class SoarManager {
    [DllImport("SoarUnityAPI")]
    public static extern IntPtr createKernel();

    [DllImport("SoarUnityAPI")]
    public static extern IntPtr createAgent(string name, IntPtr kernel);

    [DllImport("SoarUnityAPI")]
    public static extern int loadProductions(IntPtr agent, string path);

    //##################### Manage IO ######################
    [DllImport("SoarUnityAPI")]
	public static extern IntPtr getInputLink(IntPtr agent);

	//##################### Manage WMEs ######################
    [DllImport("SoarUnityAPI")]
	public static extern IntPtr createIdWME(IntPtr agent, IntPtr parent, string pAtribute);

    [DllImport("SoarUnityAPI")]
	public static extern IntPtr createStringWME(IntPtr agent, IntPtr parent, string pAtribute, string pValue);
	
    [DllImport("SoarUnityAPI")]
	public static extern IntPtr createIntWME(IntPtr agent, IntPtr parent, string pAtribute, long pValue);
	
    [DllImport("SoarUnityAPI")]
	public static extern IntPtr createFloatWME(IntPtr agent, IntPtr parent, string pAtribute, double pValue);

    [DllImport("SoarUnityAPI")]
	public static extern void commit(IntPtr agent);

	//##################### Run Agent ######################
    [DllImport("SoarUnityAPI")]
	public static extern void runSelfTilOutput(IntPtr agent);

	//##################### Events ######################
    [DllImport("SoarUnityAPI")]
	public static extern int registerForPrintEvent(IntPtr agent);

    [DllImport("SoarUnityAPI")]
	public static extern int registerForProductionAddedEvent(IntPtr agent);
}

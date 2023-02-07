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
    [DllImport("SoarUnityAPI")]
    public static extern  void setAutoCommit(IntPtr Kernel, bool state);

	//##################### Run Agent ######################
    [DllImport("SoarUnityAPI")]
	public static extern void runSelfTilOutput(IntPtr agent);

    [DllImport("SoarUnityAPI")]
	public static extern void runSelfForever(IntPtr agent);

	//##################### Events ######################
    //TODO: Show text on the screen instead of printing int the console due to formating
    [DllImport("SoarUnityAPI")]
	public static extern int registerForPrintEvent(IntPtr agent);

    //TODO not working
    // [DllImport("SoarUnityAPI")]
	// public static extern int registerForProductionAddedEvent(IntPtr agent);
	
    //##################### Debug ######################
    //TODO not working
    // [DllImport("SoarUnityAPI")]
    // public static extern bool spawnDebugger(IntPtr agent, int port=-1, string jarpath="");
    // [DllImport("SoarUnityAPI")]
	// public static extern bool killDebugger(IntPtr agent);
}

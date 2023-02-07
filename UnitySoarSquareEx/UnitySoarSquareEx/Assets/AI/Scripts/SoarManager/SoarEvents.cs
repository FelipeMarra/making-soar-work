using System;
using System.Runtime.InteropServices;

public class SoarEvents  {
	//##################### Events ######################
    //## Print Event
    //TODO: Show text on the screen instead of printing int the console due to formating
    public enum smlPrintEventId{
        smlPRINT_EVENT_BAD = -1,
        smlEVENT_ECHO = 46,
        smlEVENT_FIRST_PRINT_EVENT = 46,
        smlEVENT_PRINT = 47,
        smlEVENT_LAST_PRINT_EVENT = 47
    }
    public delegate void PrintEventCallback(smlPrintEventId eventID, IntPtr userData, IntPtr agent, string message);

    /// <summary>
    /// To send a object as a IntPtr use:
    /// GCHandle data = GCHandle.Alloc(YOUR_OBJECT);
    /// IntPtr dataPtr = GCHandle.ToIntPtr(userData);
    ///
    /// A pointer allocated in that way can than be type casted like:
    /// YOUR_OBJECT data = (YOUR_OBJECT_TYPE)((GCHandle)userDataPtr).Target;
    ///
    /// And don't forget to free it afeter use: data.Free()
    /// </summary>
    [DllImport("SoarUnityAPI", CallingConvention = CallingConvention.Cdecl)]
	public static extern int registerForPrintEvent(IntPtr agent, smlPrintEventId eventID, PrintEventCallback handler, IntPtr userData, bool ignoreOwnEchos = true, bool addToBack = true);

    //##################### Events ######################
    //TODO not working
    // [DllImport("SoarUnityAPI")]
	// public static extern int registerForProductionAddedEvent(IntPtr agent);
}

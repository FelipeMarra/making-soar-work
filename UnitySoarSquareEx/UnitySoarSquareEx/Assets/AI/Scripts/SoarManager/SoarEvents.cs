using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// To send a object as a IntPtr in the userData parameters use:
/// GCHandle data = GCHandle.Alloc(YOUR_OBJECT);
/// IntPtr dataPtr = GCHandle.ToIntPtr(userData);
///
/// A pointer allocated in that way can than be type casted like:
/// YOUR_OBJECT data = (YOUR_OBJECT_TYPE)((GCHandle)userDataPtr).Target;
///
/// And don't forget to free it afeter use: data.Free()
/// </summary>
public class SoarEvents  {
    public enum EventType{
        PRINT = 0,
        UPDATE = 1
    }

    public struct EventData {
        public GCHandle userData;
        public int eventId;
        public EventType eventType;

        public EventData(GCHandle userData, int eventId, EventType eventType){
            this.userData = userData;
            this.eventId = eventId;
            this.eventType = eventType;
        }
    }

    public static void UnregisterForEvents(List<EventData> events, IntPtr pAngent, IntPtr pKernel) {
        foreach (EventData eventData in events) {
            eventData.userData.Free();

            switch (eventData.eventType) {
                case EventType.PRINT:
                    unregisterForPrintEvent(pAngent, eventData.eventId);
                    break;
                case EventType.UPDATE:
                    unregisterForUpdateEvent(pKernel, eventData.eventId);
                    break;
            }
        }
    }

	//##################### Print ######################
    #region Print
    //TODO: Show text on the screen instead of printing into the console for better visualization
    public enum smlPrintEventId{
        smlPRINT_EVENT_BAD = -1,
        smlEVENT_ECHO = 46,
        smlEVENT_FIRST_PRINT_EVENT = 46,
        smlEVENT_PRINT = 47,
        smlEVENT_LAST_PRINT_EVENT = 47
    }
    public delegate void PrintEventHandler(smlPrintEventId eventID, IntPtr pUserData, IntPtr pAgent, string message);

    [DllImport("SoarUnityAPI")]
	public static extern int registerForPrintEvent(IntPtr pAgent, smlPrintEventId eventID, PrintEventHandler handler, IntPtr pUserData, bool ignoreOwnEchos = true, bool addToBack = true);

    [DllImport("SoarUnityAPI")]
    public static extern bool unregisterForPrintEvent(IntPtr pAgent, int callbackId);

    #endregion

    //##################### Update ######################
    #region Update
    public enum smlUpdateEventId {
        smlUPDATE_EVENT_BAD = -1,
        smlEVENT_AFTER_ALL_OUTPUT_PHASES = 53,
        smlEVENT_AFTER_ALL_GENERATED_OUTPUT = 54,
        smlEVENT_LAST_UPDATE_EVENT = 54
    }
    public enum smlRunFlags {
        sml_NONE = 0,
        sml_RUN_SELF = 1,
        sml_RUN_ALL = 2,
        sml_UPDATE_WORLD = 4,
        sml_DONT_UPDATE_WORLD = 8
    }
    public delegate void UpdateEventHandler(smlUpdateEventId eventID, IntPtr pUserData, IntPtr pKernel, smlRunFlags runFlags);

    [DllImport("SoarUnityAPI")]
    public static extern int registerForUpdateEvent(IntPtr pKernel, smlUpdateEventId id, UpdateEventHandler handler, IntPtr pUserData, bool addToBack = true);

    [DllImport("SoarUnityAPI")]
    public static extern bool unregisterForUpdateEvent(IntPtr pKernel, int callbackId);

    #endregion
    //##################### Events ######################
    //TODO not working (dont work even in pure C++)
    // [DllImport("SoarUnityAPI")]
	// public static extern int registerForProductionAddedEvent(IntPtr agent);
}

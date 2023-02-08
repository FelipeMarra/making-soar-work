using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace smlUnity{
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

    public class SoarUtils  {
        public static void UnregisterForEvents(List<EventData> events, Agent agent, Kernel kernel) {
            foreach (EventData eventData in events) {
                eventData.userData.Free();

                switch (eventData.eventType) {
                    case EventType.PRINT:
                        agent.UnregisterForPrintEvent(eventData.eventId);
                        break;
                    case EventType.UPDATE:
                        kernel.UnregisterForUpdateEvent(eventData.eventId);
                        break;
                }
            }
        }
    }
}

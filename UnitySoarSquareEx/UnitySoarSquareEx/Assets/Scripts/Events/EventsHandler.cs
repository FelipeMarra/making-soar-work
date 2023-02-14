using System;
using System.Collections.Generic;
using smlUnity;

public static class EventHandler {
    public static event Action<List<Identifier>> CommandEvent;

    public static void CallCommandEvent(List<Identifier> commands) {
        if(CommandEvent != null) {
            CommandEvent(commands);
        }
    }

    public static event Action<Agent, IntPtr, IntPtr, IntPtr, IntPtr> UpdateBlockEvent;

    public static void CallUpdateBlockEvent(Agent agent, IntPtr northId, IntPtr eastId, IntPtr southId, IntPtr westId) {
        if(UpdateBlockEvent != null) {
            UpdateBlockEvent(agent, northId, eastId, southId, westId);
        }
    }
}
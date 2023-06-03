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

    public static event Action UpdateBlockEvent;

    public static void CallUpdateBlockEvent() {
        if(UpdateBlockEvent != null) {
            UpdateBlockEvent();
        }
    }
}
using System;
using System.Collections.Generic;

public static class EventHandler {
    public static event Action<List<SoarCmd>> CommandEvent;

    public static void CallCommandEvent(List<SoarCmd> cmds) {
        if(CommandEvent != null) {
            CommandEvent(cmds);
        }
    }
}
using System.Collections.Generic;

public class AgentCmdManager : SingletonMonobehavior<AgentCmdManager> {
    private List<SoarCmd> _cmds;

    private SoarCmd _currentCmd = null;
    public SoarCmd currentCmd {
        get{return _currentCmd;}
    }

    public SoarCmdType currentCmdType {
        get{return _currentCmd.type;}
    }

    protected override void Awake() {
        base.Awake();
        _currentCmd = new SoarCmd();
    }

    private void OnEnable() {
        EventHandler.CommandEvent += SortAndRunCmds;
    }

    private void OnDisable() {
        EventHandler.CommandEvent -= SortAndRunCmds;
    }

    public void SortAndRunCmds(List<SoarCmd> cmds) {
        _cmds = cmds;
        _cmds.Sort((e1,e2) => {
            if(e1.priority > e2.priority){
                return 1;
            }
            if(e1.priority < e2.priority){
                return -1;
            }
            return 0;
        });
        RunFirstCmd();
    }

    public void RunFirstCmd() {
        _currentCmd = _cmds[0];
        _currentCmd.Run();
    }

    public void AddCompleteAndResetCommand() {
        if(_currentCmd.type != SoarCmdType.none) {
            _currentCmd.AddStatusComplete();
            ResetCommand();
        }
    }

    public void AddErrorAndResetCommand() {
        _currentCmd.AddStatusError();
        ResetCommand();
    }

    private void ResetCommand() {
        _currentCmd.Reset();
        _currentCmd = new SoarCmd();

        _cmds.RemoveAt(0);
        if(_cmds.Count > 0) {
            RunFirstCmd();
        } else {
            SquareAgent.Instance.UnlockAgent();
        }
    }
}

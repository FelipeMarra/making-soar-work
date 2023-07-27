using System;
using smlUnity;

public class SoarCmd {
    protected Identifier _cmdId;
    private string _name;
    public string name {
        get{return _name;}
    }
    public bool isStatusComplete {
        get{return _cmdId.GetParameterValue("status") == "complete";}
    }
    SoarCmdType _type;
    public SoarCmdType type {
        get{return _type;}
    }
    protected int _priority;
    public int priority {
        get{return _priority;}
    }

    public SoarCmd(Identifier cmdId) {
        this._cmdId = cmdId;
        this._name = cmdId.GetCommandName();
        this._type = GetTypeFromIdentifier(cmdId);
    }

    public SoarCmd() {
        this._cmdId = null;
        this._name = "none";
        this._type = SoarCmdType.none;
    }

    public bool IsOfType(SoarCmdType cmdType) {
        return cmdType == type;
    }

    public static SoarCmdType GetTypeFromIdentifier(Identifier id) {
        SoarCmdType cmdType;
        Enum.TryParse<SoarCmdType>(id.GetCommandName(), out cmdType);
        return cmdType;
    }

    public virtual void Run(){
        throw new Exception("Implement Soar Run Cmd");
    }

    public virtual void Reset(){
        throw new Exception("Implement Soar Reset Cmd");
    }

    public void AddStatusComplete() {
        _cmdId.AddStatusComplete();
    }

    public void AddStatusError() {
        _cmdId.AddStatusError();
    }
}
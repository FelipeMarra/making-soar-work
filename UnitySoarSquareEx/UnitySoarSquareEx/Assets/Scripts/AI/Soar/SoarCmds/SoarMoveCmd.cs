using UnityEngine;
using smlUnity;

public class SoarMoveCmd : SoarCmd {
    public SoarMoveCmd(Identifier cmdId) : base(cmdId){
        this._priority = 1;
    }

    public override void Run() {
        string directionName = _cmdId.GetParameterValue("direction");

        Debug.Log("<color=lightblue> RUNNING MOVE TO " + directionName + " </color>");

        Vector3 directionVec = new Vector3();
        switch (directionName) {
            case "north":
                directionVec = new Vector3(0f,1f,0f);
                break;
            case "east":
                directionVec = new Vector3(1f,0f,0f);
                break;
            case "south":
                directionVec = new Vector3(0f,-1f,0f);
                break;
            case "west":
                directionVec = new Vector3(-1f,0f,0f);
                break;
        }

        SquareMovement.Instance.MovePlayer(directionVec);
    }

    public override void Reset() {
        Debug.Log("<color=green> MOVIMENT FINALIZED </color>");
    }
}
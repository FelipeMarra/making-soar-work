using System.Collections.Generic;
using UnityEngine;
using smlUnity;

public class SquareMovement : MonoBehaviour {
    public float speed;

    private void OnEnable() {
        EventHandler.CommandEvent += ExecuteCommands;
    }

    private void OnDisable() {
        EventHandler.CommandEvent -= ExecuteCommands;
    }

    void ExecuteCommands(List<Identifier> commands) {
        foreach (Identifier cmd in commands) {
            string cmdName = cmd.GetCommandName();

            if(cmdName == "move") {
                string directionName = cmd.GetParameterValue("direction");
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

                this.transform.position += directionVec * speed * Time.deltaTime;

                cmd.AddStatusComplete();
            }
        }
    }

}

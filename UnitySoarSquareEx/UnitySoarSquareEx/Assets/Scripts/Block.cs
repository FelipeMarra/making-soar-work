using System;
using UnityEngine;
using smlUnity;

public class Block : MonoBehaviour {

    private string northValue = "no", eastValue = "no", southValue = "no", westValue = "no";

    private void OnEnable() {
        EventHandler.UpdateBlockEvent += UpdateBlock;
    }

    private void OnDisable() {
        EventHandler.UpdateBlockEvent -= UpdateBlock;
    }

    void OnTriggerEnter2D(Collider2D other) {
        switch (other.gameObject.tag){
            case "North":
                northValue = "yes";
                Debug.Log("<color=red> Collided NORTH </color>");
                break;
            case "East":
                eastValue = "yes";
                Debug.Log("<color=red> Collided EAST </color>");
                break;
            case "South":
                southValue = "yes";
                Debug.Log("<color=red> Collided SOUTH </color>");
                break;
            case "West":
                westValue = "yes";
                Debug.Log("<color=red> Collided WEST </color>");
                break;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        switch (other.gameObject.tag){
            case "North":
                northValue = "no";
                break;
            case "East":
                eastValue = "no";
                break;
            case "South":
                southValue = "no";
                break;
            case "West":
                westValue = "no";
                break;
            
        }
    }

    void UpdateBlock(Agent agent, IntPtr northId, IntPtr eastId, IntPtr southId, IntPtr westId) {
        agent.Update(northId, northValue);
        agent.Update(eastId, eastValue);
        agent.Update(southId, southValue);
        agent.Update(westId, westValue);
        agent.Commit();
    }
}

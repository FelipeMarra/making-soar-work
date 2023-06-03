using System;
using UnityEngine;
using smlUnity;

public class BlockSquareMovement : MonoBehaviour {


    void OnTriggerEnter2D(Collider2D other) {
        switch (other.gameObject.tag){
            case "North":
                SquareAgent.Instance.blockValues[0] = "yes";
                Debug.Log("<color=red> Collided NORTH </color>");
                break;
            case "East":
                SquareAgent.Instance.blockValues[1] = "yes";
                Debug.Log("<color=red> Collided EAST </color>");
                break;
            case "South":
                SquareAgent.Instance.blockValues[2] = "yes";
                Debug.Log("<color=red> Collided SOUTH </color>");
                break;
            case "West":
                SquareAgent.Instance.blockValues[3] = "yes";
                Debug.Log("<color=red> Collided WEST </color>");
                break;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        switch (other.gameObject.tag){
            case "North":
                SquareAgent.Instance.blockValues[0] = "no";
                break;
            case "East":
                SquareAgent.Instance.blockValues[1] = "no";
                break;
            case "South":
                SquareAgent.Instance.blockValues[2] = "no";
                break;
            case "West":
                SquareAgent.Instance.blockValues[3] = "no";
                break;
            
        }
    }
}

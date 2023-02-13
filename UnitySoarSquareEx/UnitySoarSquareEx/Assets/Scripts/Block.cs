using UnityEngine;

public class Block : MonoBehaviour
{
    public float range;

    void Update() {
        DistanceCheck();
    }

    private void DistanceCheck() {
        //TODO
        if(Vector3.Distance(SquareAgent.Instance.transform.position, transform.position) < range){
            
        } else {

        }
    }
}

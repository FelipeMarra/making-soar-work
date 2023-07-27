using UnityEngine;

public class SquareMovement : SingletonMonobehavior<SquareMovement> {
    public float speed;

    public void MovePlayer(Vector3 direction) {
        this.transform.position += direction * speed * Time.deltaTime;

        AgentCmdManager.Instance.AddCompleteAndResetCommand();
    }
}

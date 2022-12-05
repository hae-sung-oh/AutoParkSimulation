using UnityEngine;

public class ParkingLot : MonoBehaviour {
    public bool IsOccupied { get; set; }
    public Vector3 Orientation => transform.forward;
    private float alignment = 0f;
    private float distance = 0f;
    public AutoParkAgent agent;

    private void Awake() {

    }

    private void CheckPosition() {
        if (distance != Vector3.Distance(gameObject.transform.position, agent.gameObject.transform.position)) {
            distance = Vector3.Distance(gameObject.transform.position, agent.gameObject.transform.position);
        }
        if (alignment != Vector3.Dot(gameObject.transform.right, agent.gameObject.transform.forward)) {
            alignment = Vector3.Dot(gameObject.transform.right, agent.gameObject.transform.forward);
        }
        if (Mathf.Abs(alignment) > 0.9 && distance < 0.5) 
            agent.StartCoroutine(agent.JackpotReward(Mathf.Abs(alignment)/distance));
    }

    private void Update() {
        CheckPosition();
    }
}

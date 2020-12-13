using GOAP;
using UnityEngine;

public class DangerSensor : MonoBehaviour {

    private Agent agentToUpdate;
    [SerializeField]
    private LayerMask dangerLayerMask;
    [SerializeField]
    private WorldStateKey inDangerKey;
    private WorldState inDangerWSTracked;

    private void Awake() {

        agentToUpdate = GetComponent<Agent>();
        inDangerWSTracked = new WorldState(inDangerKey, false);
        agentToUpdate.UpdatePerception(inDangerWSTracked);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(dangerLayerMask.ContainsLayer(collision.gameObject.layer)) {
            SetDanger(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (dangerLayerMask.ContainsLayer(collision.gameObject.layer)) {
            SetDanger(false);
        }
    }

    private void SetDanger(bool inDanger) {
        inDangerWSTracked.BoolValue = inDanger;
        agentToUpdate.UpdatePerception(inDangerWSTracked);
    }

}


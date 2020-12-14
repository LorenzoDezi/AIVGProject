using GOAP;
using System.Collections;
using UnityEngine;

public class DangerSensor : MonoBehaviour {

    private Agent agentToUpdate;
    [SerializeField]
    private LayerMask dangerLayerMask;
    [SerializeField]
    private WorldStateKey inDangerKey;
    private WorldState inDangerWSTracked;
    [SerializeField]
    private WorldStateKey dangerAroundKey;
    private WorldState dangerAroundWSTracked;

    private int dangerCount;
    public float DangerRadius { get; private set; }
    public Vector3 DangerSource { get; private set; }

    private void Awake() {

        agentToUpdate = GetComponent<Agent>();
        inDangerWSTracked = new WorldState(inDangerKey, false);
        agentToUpdate.UpdatePerception(inDangerWSTracked);
        dangerAroundWSTracked = new WorldState(dangerAroundKey, false);
        agentToUpdate.UpdatePerception(dangerAroundWSTracked);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(dangerLayerMask.ContainsLayer(collision.gameObject.layer)) {
            //the last danger is the one took into consideration
            var danger = collision.GetComponent<IDangerous>();
            DangerRadius = danger.DangerRadius;
            DangerSource = danger.DangerSource;
            danger.DangerEnd.AddListener(OnDangerEnd);
            SetInDanger(true);
            SetDangerAround(true);
            dangerCount++;
        }
    }

    private void OnDangerEnd(IDangerous danger) {
        dangerCount--;
        danger.DangerEnd.RemoveListener(OnDangerEnd);
        if(dangerCount == 0) {
            DangerRadius = 0f;
            DangerSource = Vector3.zero;
            SetDangerAround(false);
            SetInDanger(false);
        }           
    }

    private void SetInDanger(bool inDanger) {
        inDangerWSTracked.BoolValue = inDanger;
        agentToUpdate.UpdatePerception(inDangerWSTracked);
    }

    private void SetDangerAround(bool dangerAround) {
        dangerAroundWSTracked.BoolValue = dangerAround;
        agentToUpdate.UpdatePerception(dangerAroundWSTracked);
    }

}


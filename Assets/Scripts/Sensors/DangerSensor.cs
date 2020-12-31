using GOAP;
using System.Collections;
using UnityEngine;

public class DangerSensor : MonoBehaviour {

    private Agent agentToUpdate;
    private new Transform transform;

    [SerializeField]
    private LayerMask obstacleMask;
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
        transform = GetComponent<Transform>();

        inDangerWSTracked = new WorldState(inDangerKey, false);
        agentToUpdate.UpdatePerception(inDangerWSTracked);
        dangerAroundWSTracked = new WorldState(dangerAroundKey, false);
        agentToUpdate.UpdatePerception(dangerAroundWSTracked);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(dangerLayerMask.ContainsLayer(collision.gameObject.layer)) {
            var danger = collision.GetComponent<IDangerous>();            
            if(!Physics2D.Linecast(danger.DangerSource, transform.position, obstacleMask)) {
                //the last danger is the one took into consideration
                DangerRadius = danger.DangerRadius;
                DangerSource = danger.DangerSource;
                SetInDanger(true);
            }
            danger.DangerEnd += OnDangerEnd;
            SetDangerAround(true);
            dangerCount++;
        }
    }

    private void OnDangerEnd(IDangerous danger) {
        dangerCount--;
        danger.DangerEnd -= OnDangerEnd;
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


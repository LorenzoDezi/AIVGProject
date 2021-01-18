using GOAP;
using System.Collections;
using UnityEngine;

public class DangerSensor : MonoBehaviour {

    private Agent agentToUpdate;
    private EnemyVisualSensor visualSensor;
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

    [SerializeField]
    private float timeToCheckDanger = 1f;
    private float currTimeToCheckDanger;

    private void Awake() {

        agentToUpdate = GetComponent<Agent>();
        visualSensor = GetComponent<EnemyVisualSensor>();
        transform = GetComponent<Transform>();

        inDangerWSTracked = new WorldState(inDangerKey, false);
        agentToUpdate.UpdatePerception(inDangerWSTracked);
        dangerAroundWSTracked = new WorldState(dangerAroundKey, false);
        agentToUpdate.UpdatePerception(dangerAroundWSTracked);

        currTimeToCheckDanger = timeToCheckDanger;
    }

    private void Update() {
        if(currTimeToCheckDanger >= timeToCheckDanger) {
            var dangerObj = visualSensor.CheckInConeOfVision(dangerLayerMask);
            if(dangerObj != null)
                ProcessDanger(dangerObj);
            currTimeToCheckDanger = 0f;
        } else {
            currTimeToCheckDanger += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(dangerLayerMask.ContainsLayer(collision.gameObject.layer)) {
            ProcessDanger(collision.gameObject);
        }
    }

    private void ProcessDanger(GameObject dangerObj) {
        var danger = dangerObj.GetComponent<IDangerous>();
        if (!Physics2D.Linecast(danger.DangerSource, transform.position, obstacleMask)) {
            //the last danger is the one took into consideration
            //TODO: A list of dangers? And then?
            DangerRadius = danger.DangerRadius;
            DangerSource = danger.DangerSource;
            SetInDanger(true);
        }
        //TODO: if we make a squad to keep track of the grenades, we can do better...
        //in particular, dangerEnd when all the grenades are exploded, this 
        //leads to enemy walking on the grenade after only the first one has exploded
        //TODO: block some actions with DangerAround e sbloccalo quando tutti i pericoli
        //se ne sono andati
        danger.DangerEnd += OnDangerEnd;
        SetDangerAround(true);
        dangerCount++;
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


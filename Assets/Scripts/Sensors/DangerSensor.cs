using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerSensor : MonoBehaviour {

    private Agent agentToUpdate;
    private EnemyVisualSensor visualSensor;
    private GrenadeController grenadeController;
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

    private List<IDangerous> dangers;
    public float DangerRadius { get; private set; }
    public Vector3 DangerSource { get; private set; }

    [SerializeField]
    private float timeToCheckDanger = 1f;
    private float currTimeToCheckDanger;

    private bool canLaunchGrenades;

    private event DangerFoundHandler dangerFound;
    public event DangerFoundHandler DangerFound {
        add {
            if(canLaunchGrenades)
                grenadeController.GrenadeLaunched += value;
            dangerFound += value;
        }
        remove {
            if(canLaunchGrenades)
                grenadeController.GrenadeLaunched -= value;
            dangerFound -= value;
        }
    }

    #region monobehaviour methods
    private void Awake() {

        agentToUpdate = GetComponent<Agent>();
        visualSensor = GetComponent<EnemyVisualSensor>();
        transform = GetComponent<Transform>();
        grenadeController = GetComponentInChildren<GrenadeController>();
        if (grenadeController != null)
            canLaunchGrenades = true;

        inDangerWSTracked = new WorldState(inDangerKey, false);
        agentToUpdate.UpdatePerception(inDangerWSTracked);
        dangerAroundWSTracked = new WorldState(dangerAroundKey, false);
        agentToUpdate.UpdatePerception(dangerAroundWSTracked);

        dangers = new List<IDangerous>();

        currTimeToCheckDanger = timeToCheckDanger;
    }

    private void Update() {
        if (currTimeToCheckDanger >= timeToCheckDanger) {
            var dangerObj = visualSensor.CheckInConeOfVision(dangerLayerMask);
            if (dangerObj != null)
                ProcessDanger(dangerObj);
            currTimeToCheckDanger = 0f;
        } else {
            currTimeToCheckDanger += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (dangerLayerMask.ContainsLayer(collision.gameObject.layer)) {
            ProcessDanger(collision.gameObject);
        }
    }
    #endregion

    #region public methods
    public void RegisterDanger(IDangerous danger) {
        if (!dangers.Contains(danger)) {
            danger.DangerEnd += OnDangerEnd;
            SetDangerAround(true);
            dangers.Add(danger);
        }
    }


    public void SetInDanger(bool inDanger) {
        inDangerWSTracked.BoolValue = inDanger;
        agentToUpdate.UpdatePerception(inDangerWSTracked);
    }
    #endregion

    #region private methods
    private void ProcessDanger(GameObject dangerObj) {
        var danger = dangerObj.GetComponent<IDangerous>();
        if (!Physics2D.Linecast(danger.DangerSource, transform.position, obstacleMask)) {
            DangerRadius = danger.DangerRadius;
            DangerSource = danger.DangerSource;
            SetInDanger(true);
        }
        RegisterDanger(danger);
        dangerFound?.Invoke(danger);
    }

    private void OnDangerEnd(IDangerous danger) {
        dangers.Remove(danger);
        danger.DangerEnd -= OnDangerEnd;
        if (dangers.Count == 0) {
            DangerRadius = 0f;
            DangerSource = Vector3.zero;
            SetDangerAround(false);
            SetInDanger(false);
        }
    }

    private void SetDangerAround(bool dangerAround) {
        dangerAroundWSTracked.BoolValue = dangerAround;
        agentToUpdate.UpdatePerception(dangerAroundWSTracked);
    } 
    #endregion

}


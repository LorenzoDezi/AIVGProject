using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOAP;
using UnityEngine;

[CreateAssetMenu(fileName = "StayAliveAction", menuName = "GOAP/Actions/StayAliveAction")]
public class RefillHealthAction : GOAP.Action {

    //TODO : SPOSTARE TUTTA LA LOGICA DELLA RICERCA SU HEALTHSENSOR

    private NavigationComponent navigationComponent;
    private CharacterController charController;
    private HealthComponent healthComponent;
    private Transform transform;

    private bool hasNearHealthStation;
    private HealthStation nearestHealthStation;
    private HealthStation currentHealthStation;

    [Header("HealthStation check parameters")]
    [SerializeField]
    private float checkForHealthStationsRadius = 5f;
    [SerializeField]
    private int checkHSMaxQueryResults = 5;  
    [SerializeField]
    private LayerMask HSLayer;
    [SerializeField]
    private LayerMask wallLayerMask;
    private Collider2D[] cachedCheckHSResults;

    [Header("WorldState fields")]
    [SerializeField]
    private WorldStateKey needCoverKey;
    private WorldState needCoverWS;

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);

        navigationComponent = agentGameObj.GetComponent<NavigationComponent>();
        transform = agentGameObj.GetComponent<Transform>();
        charController = agentGameObj.GetComponent<CharacterController>();
        healthComponent = agentGameObj.GetComponent<HealthComponent>();

        needCoverWS = agent[needCoverKey];
        if(needCoverWS == null) {
            needCoverWS = new WorldState(needCoverKey, false);
            agent.Add(needCoverWS);
        }

        cachedCheckHSResults = new Collider2D[checkHSMaxQueryResults];
    }

    public override bool CheckProceduralConditions() {

        if (healthComponent.CurrHealth == healthComponent.MaxHealth)
            return false;

        var foundHealthStation = GetBestHealthStation();
        hasNearHealthStation = foundHealthStation != null;

        if(hasNearHealthStation) {
            nearestHealthStation = foundHealthStation;
        }
        return hasNearHealthStation;
    }

    private HealthStation GetBestHealthStation() {

        int resultCount = Physics2D.OverlapCircleNonAlloc(
            transform.position, checkForHealthStationsRadius,
            cachedCheckHSResults, HSLayer
        );

        float sqrMinDistance = float.PositiveInfinity;
        HealthStation result = null;
        for(int i = 0; i < resultCount && i < checkHSMaxQueryResults; i++) {

            HealthStation current = cachedCheckHSResults[i].GetComponent<HealthStation>();
            if (current == null || 
                !current.CanRefill)
                continue;

            float currSqrDist = Vector3.SqrMagnitude(
                cachedCheckHSResults[i].transform.position - transform.position
            );
            if(currSqrDist < sqrMinDistance) {
                result = current;
                sqrMinDistance = currSqrDist;
            }            
        }
        return result;
    }

    public override bool Activate() {

        if (!hasNearHealthStation)
            return false;

        needCoverWS.BoolValue = true;
        navigationComponent.MoveTo(nearestHealthStation.Transform.position);
        AddListeners();
        return true;
    }

    private void Refill(bool success) {
        RemoveListeners();
        if (success && nearestHealthStation.UseRefill()) {
            healthComponent.Restore(healthComponent.MaxHealth);
        }      
        Terminate(success);
    }

    private void OnRefillsEmpty() {
        RemoveListeners();
        Terminate(false);
    }

    public override void Deactivate() {
        needCoverWS.BoolValue = false;
        RemoveListeners();
    }

    private void AddListeners() {
        nearestHealthStation.RefillsEmpty += OnRefillsEmpty;
        navigationComponent.PathCompleted += Refill;
    }

    private void RemoveListeners() {
        navigationComponent.PathCompleted -= Refill;
        nearestHealthStation.RefillsEmpty -= OnRefillsEmpty;
    }

    public override void Update() {
        charController.AimAt(transform.position + navigationComponent.DirectionToWaypoint);
    }

}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOAP;
using UnityEngine;

[CreateAssetMenu(fileName = "StayAliveAction", menuName = "GOAP/Actions/StayAliveAction")]
public class RefillHealthAction : GOAP.Action {

    private NavigationComponent navigationComponent;
    private HealthComponent healthComponent;
    private Transform transform;

    private bool hasNearHealthStation;
    private HealthStation nearestHealthStation;

    [Header("HealthStation check parameters")]
    [SerializeField]
    private float checkForHealthStationsRadius = 5f;
    [SerializeField]
    private int checkHSMaxQueryResults = 5;  
    [SerializeField]
    private LayerMask HSLayer;
    private Collider2D[] cachedCheckHSResults;

    [Header("WorldState fields")]
    [SerializeField]
    private WorldStateKey needCoverKey;
    private WorldState needCoverWS;

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);

        navigationComponent = agentGameObj.GetComponent<NavigationComponent>();
        transform = agentGameObj.GetComponent<Transform>();
        healthComponent = agentGameObj.GetComponent<HealthComponent>();

        needCoverWS = new WorldState(needCoverKey, false);

        cachedCheckHSResults = new Collider2D[checkHSMaxQueryResults];
    }

    public override bool CheckProceduralConditions() {

        if (healthComponent.CurrHealth == healthComponent.MaxHealth)
            return false;

        var hsCollider = GetBestHealthStation();
        hasNearHealthStation = hsCollider != null;

        if(hasNearHealthStation)
            nearestHealthStation = hsCollider.GetComponent<HealthStation>();

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

            if (!current.CanRefill)
                break;

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

        UpdateNeedCoverWS(true);
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
        UpdateNeedCoverWS(false);
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

    private void UpdateNeedCoverWS(bool value) {
        needCoverWS.BoolValue = value;
        agent.UpdatePerception(needCoverWS);
    }

    public override void Update() { }

}


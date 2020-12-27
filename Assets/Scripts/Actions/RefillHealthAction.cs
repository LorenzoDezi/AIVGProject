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
    private Transform nearestHealthStation;

    [Header("Health Station Check parameters")]
    [SerializeField]
    private float checkForHealthStationsRadius = 5f;
    [SerializeField]
    private int checkHSMaxQueryResults = 5;  
    [SerializeField]
    private LayerMask HSLayer;
    private ContactFilter2D healthLayerContactFilter;
    private Collider2D[] checkHealthStationResults;

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);

        navigationComponent = agentGameObj.GetComponent<NavigationComponent>();
        transform = agentGameObj.GetComponent<Transform>();
        healthComponent = agentGameObj.GetComponent<HealthComponent>();

        healthLayerContactFilter = new ContactFilter2D();
        healthLayerContactFilter.SetLayerMask(HSLayer);
        healthLayerContactFilter.useTriggers = true;
        checkHealthStationResults = new Collider2D[checkHSMaxQueryResults];
    }

    public override bool CheckProceduralConditions() {

        if (healthComponent.CurrHealth == healthComponent.MaxHealth)
            return false;

        var hsCollider = GetBestHSCollider();
        hasNearHealthStation = hsCollider != null;

        if(hasNearHealthStation)
            nearestHealthStation = hsCollider.transform;

        return hasNearHealthStation;
    }

    private Collider2D GetBestHSCollider() {

        int resultCount = Physics2D.OverlapCircle(
            transform.position, checkForHealthStationsRadius,
            healthLayerContactFilter, checkHealthStationResults
        );
        Debug.LogWarningFormat("Result Count HS {0}", resultCount);
        float sqrMinDistance = float.PositiveInfinity;
        Collider2D result = null;
        for(int i = 0; i < resultCount && i < checkHSMaxQueryResults; i++) {
            float currSqrDist = Vector3.SqrMagnitude(
                checkHealthStationResults[i].transform.position - transform.position
            );
            if(currSqrDist < sqrMinDistance) {
                result = checkHealthStationResults[i];
                sqrMinDistance = currSqrDist;
            }            
        }

        return result;
    }

    public override bool Activate() {

        if (!hasNearHealthStation)
            return false;

        navigationComponent.MoveTo(nearestHealthStation.position);
        navigationComponent.PathCompleted.AddListener(Refill);
        return true;
    }

    private void Refill(bool success) {

        navigationComponent.PathCompleted.RemoveListener(Refill);
        if (success) {
            healthComponent.Restore(healthComponent.MaxHealth);
        }      
        Terminate(success);
    }

    public override void Deactivate() {
        navigationComponent.PathCompleted.RemoveListener(Refill);
    }

    public override void Update() { }

}


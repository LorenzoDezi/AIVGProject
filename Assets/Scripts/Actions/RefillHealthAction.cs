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
    private Transform nearestHealthStation;
    //TODO Avoid all this serialized fields with key
    [SerializeField]
    private WorldStateKey healthRefilledKey;

    [SerializeField]
    private float checkForHealthStationsRadius = 5f;
    [SerializeField]
    private LayerMask healthStationLayer;

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);
        navigationComponent = agentGameObj.GetComponent<NavigationComponent>();
        healthComponent = agentGameObj.GetComponent<HealthComponent>();
    }

    public override bool CheckProceduralConditions() {
        if (healthComponent.CurrHealth == healthComponent.MaxHealth)
            return false;
        nearestHealthStation = Physics2D.OverlapCircle(
            navigationComponent.transform.position, checkForHealthStationsRadius, healthStationLayer
        )?.transform;
        Debug.LogFormat("CheckProceduralConditions on RefHAction {0}", 
            nearestHealthStation != null);
        return nearestHealthStation != null;
    }

    public override void Activate() {
        if (nearestHealthStation != null) {
            navigationComponent.MoveTo(nearestHealthStation);
            navigationComponent.PathCompleted.AddListener(Refill);
        }
    }

    private void Refill() {
        healthComponent.Restore(healthComponent.MaxHealth);
        navigationComponent.PathCompleted.RemoveListener(Refill);
        Terminate();
    }

    public override void Deactivate() { }

    public override void Update() { }

}


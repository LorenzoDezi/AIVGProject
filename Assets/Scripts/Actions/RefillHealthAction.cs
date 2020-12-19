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

    public override bool Activate() {

        if (nearestHealthStation == null)
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


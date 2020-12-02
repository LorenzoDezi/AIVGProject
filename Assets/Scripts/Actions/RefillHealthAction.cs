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
    private WorldStates worldPerception;

    [SerializeField]
    private float checkForHealthStationsRadius = 5f;
    [SerializeField]
    private LayerMask healthStationLayer;

    

    public override void Init(GameObject agentGameObj, GOAP.Action actionTemplate) {
        base.Init(agentGameObj, actionTemplate);
        navigationComponent = agentGameObj.GetComponent<NavigationComponent>();
        healthComponent = agentGameObj.GetComponent<HealthComponent>();
    }

    public override void Activate() {
        nearestHealthStation = Physics2D.OverlapCircle(
            navigationComponent.transform.position, checkForHealthStationsRadius, healthStationLayer
        )?.transform;
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


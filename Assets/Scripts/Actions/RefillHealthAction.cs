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
    private CharacterController charController;
    private HealthComponent healthComponent;
    private HealthSensor healthSensor;
    private Transform transform;

    [Header("WorldState fields")]
    [SerializeField]
    private WorldStateKey needCoverKey;
    private WorldState needCoverWS;

    private HealthStation currNearestHealthStation;

    public override bool CheckProceduralConditions() {
        return healthSensor.HasNearHealthStation;
    }

    public override void Init(GameObject agentGameObj) {
    
        navigationComponent = agentGameObj.GetComponent<NavigationComponent>();
        transform = agentGameObj.GetComponent<Transform>();
        charController = agentGameObj.GetComponent<CharacterController>();
        healthComponent = agentGameObj.GetComponent<HealthComponent>();
        healthSensor = agentGameObj.GetComponent<HealthSensor>();
        var agent = agentGameObj.GetComponent<Agent>();
        needCoverWS = agent[needCoverKey];
        if(needCoverWS == null) {
            needCoverWS = new WorldState(needCoverKey, false);
            agent.Add(needCoverWS);
        }
    }

    public override bool Activate() {

        if (!healthSensor.HasNearHealthStation)
            return false;
        needCoverWS.BoolValue = true;
        currNearestHealthStation = healthSensor.NearestHealthStation;
        navigationComponent.MoveTo(currNearestHealthStation.Transform.position);
        AddListeners();
        return true;
    }

    private void Refill(bool success) {
        RemoveListeners();
        if (success && currNearestHealthStation.UseRefill()) {
            healthComponent.Restore(healthComponent.MaxHealth);
        }      
        Terminate(success);
    }

    private void OnRefillsEmpty() {
        RemoveListeners();
        Terminate(false);
    }

    private void OnNearestHSChanged() {
        if(!healthSensor.HasNearHealthStation) {
            Terminate(false);
            return;
        }
        currNearestHealthStation.RefillsEmpty -= OnRefillsEmpty;
        currNearestHealthStation = healthSensor.NearestHealthStation;
        navigationComponent.MoveTo(currNearestHealthStation.Transform.position);
        currNearestHealthStation.RefillsEmpty += OnRefillsEmpty;
    }

    public override void Deactivate() {
        needCoverWS.BoolValue = false;
        RemoveListeners();
    }

    private void AddListeners() {
        healthSensor.NearestHSChanged += OnNearestHSChanged;
        currNearestHealthStation.RefillsEmpty += OnRefillsEmpty;
        navigationComponent.PathCompleted += Refill;
    }

    private void RemoveListeners() {
        healthSensor.NearestHSChanged -= OnNearestHSChanged;
        navigationComponent.PathCompleted -= Refill;
        currNearestHealthStation.RefillsEmpty -= OnRefillsEmpty;
    }

    public override void Update() {
        charController.AimAt(transform.position + navigationComponent.DirectionToWaypoint);
    }

}


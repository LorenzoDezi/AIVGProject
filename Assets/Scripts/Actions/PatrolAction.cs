using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOAP;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolAction", menuName = "GOAP/Actions/PatrolAction")]
public class PatrolAction : GOAP.Action {

    private PatrollerComponent patroller;

    [SerializeField]
    private WorldStateKey patrollingKey;
    private WorldState currPatrollingState;

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);
        patroller = agentGameObj.GetComponent<PatrollerComponent>();
        currPatrollingState = new WorldState(patrollingKey, false);
        agent.UpdatePerception(currPatrollingState);
    }

    public override bool Activate() {
        patroller.StartPatrol();
        SetPatrollingWorldState(true);
        return true;
    }


    public override void Update() {

    }

    public override void Deactivate() {
        patroller.StopPatrol();
        SetPatrollingWorldState(false);
    }

    private void SetPatrollingWorldState(bool value) {
        currPatrollingState.BoolValue = value;
        agent.UpdatePerception(currPatrollingState);
    }
    
}


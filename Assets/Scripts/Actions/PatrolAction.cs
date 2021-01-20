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
    private WorldStateKey IdleKey;
    private WorldState idleWSTracked;

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);
        patroller = agentGameObj.GetComponent<PatrollerComponent>();
        idleWSTracked = agent[IdleKey];
        if(idleWSTracked == null) {
            idleWSTracked = new WorldState(IdleKey, false);
            agent.Add(idleWSTracked);
        }
    }

    public override bool Activate() {
        patroller.StartPatrol();
        UpdateIdleWorldState(true);
        return true;
    }


    public override void Update() {
        patroller.UpdateAim();
    }

    public override void Deactivate() {
        patroller.StopPatrol();
        UpdateIdleWorldState(false);
    }

    private void UpdateIdleWorldState(bool value) {
        idleWSTracked.BoolValue = value;
    }
    
}


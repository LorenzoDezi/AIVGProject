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

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);
        patroller = agentGameObj.GetComponent<PatrollerComponent>();
    }

    public override bool Activate() {
        patroller.StartPatrol();
        return true;
    }


    public override void Update() {
        patroller.UpdateAim();
    }

    public override void Deactivate() {
        patroller.StopPatrol();
    }
    
}


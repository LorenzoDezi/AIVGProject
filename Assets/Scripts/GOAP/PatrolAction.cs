using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOAP;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolAction", menuName = "GOAP/Actions/PatrolAction")]
public class PatrolAction : GOAP.Action {

    private NavigationComponent navigationComp;

    public override void Init(GameObject agentGameObj) {
        navigationComp = agentGameObj.GetComponent<NavigationComponent>();
    }

    public override void Activate() {
        
    }

    

    public override void Update() {
        
    }
}


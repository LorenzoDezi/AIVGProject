using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ReloadAction", menuName = "GOAP/Actions/ReloadAction")]
public class ReloadAction : GOAP.Action {

    private GunController gunController;

    public override bool CheckProceduralConditions() {
        return !gunController.HasShotsInClip;
    }

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);
        gunController = agentGameObj.GetComponentInChildren<GunController>();
    }

    public override bool Activate() {
        gunController.Reload();
        return true;
    }

    public override void Deactivate() {
        
    }

    public override void Update() {
        Terminate(true);
    }
}


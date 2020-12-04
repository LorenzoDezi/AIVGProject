using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ShootStandingAction", menuName = "GOAP/Actions/ShootStandingAction")]
public class ShootStandingAction : GOAP.Action {

    private EnemyVisualSensor enemySensor;
    private GunController gunController;
    private CharacterController charController;
    private Transform target;

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);
        gunController = agentGameObj.GetComponentInChildren<GunController>();
        charController = agentGameObj.GetComponent<CharacterController>();
        enemySensor = agentGameObj.GetComponent<EnemyVisualSensor>();
    }

    public override void Activate() {
        target = enemySensor.EnemyVisible.transform;        
    }

    public override void Deactivate() {
        target = null;
    }

    public override void Update() {
        charController.AimAt(target.position);
    }
}


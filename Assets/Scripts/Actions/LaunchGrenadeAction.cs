using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "LaunchGrenadeAction", menuName = "GOAP/Actions/LaunchGrenadeAction")]
public class LaunchGrenadeAction : GOAP.Action {

    private GrenadeController grenadeController;
    private CharacterController charController;
    private VisualSensor enemyVisualSensor;

    private Transform target;

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);

        charController = agentGameObj.GetComponent<CharacterController>();
        grenadeController = agentGameObj.GetComponentInChildren<GrenadeController>();
        enemyVisualSensor = agentGameObj.GetComponent<VisualSensor>();
    }

    public override bool Activate() {
        if (!enemyVisualSensor.IsEnemySpotted)
            return false;
        target = enemyVisualSensor.VisibleEnemy;
        enemyVisualSensor.CurrWeaponRange = grenadeController.WeaponRange;
        return true;
    }

    public override void Deactivate() {

    }

    public override void Update() {
        charController.AimAt(target.position);
        grenadeController.LaunchAt(target.position);
    }
}


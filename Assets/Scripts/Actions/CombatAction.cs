using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ShootStandingAction", menuName = "GOAP/Actions/ShootStandingAction")]
public class CombatAction : GOAP.Action {

    protected EnemyVisualSensor enemySensor;
    protected GunController gunController;
    protected CharacterController charController;
    protected Transform target;

    [SerializeField]
    protected float shootInterval = 0.5f;
    protected float timeSinceLastShoot;

    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);

        gunController = agentGameObj.GetComponentInChildren<GunController>();
        charController = agentGameObj.GetComponent<CharacterController>();
        enemySensor = agentGameObj.GetComponent<EnemyVisualSensor>();
    }

    public override bool Activate() {

        if(enemySensor.VisibleEnemy == null)
            return false;

        target = enemySensor.VisibleEnemy.transform;
        timeSinceLastShoot = shootInterval;
        return true;
    }

    public override void Deactivate() {
        target = null;
    }

    public override void Update() {

        charController.AimAt(target.position);

        if (timeSinceLastShoot >= shootInterval) {
            gunController.TryToShoot();
            timeSinceLastShoot = 0f;
        } else {
            timeSinceLastShoot += Time.deltaTime;
        }
    }
}


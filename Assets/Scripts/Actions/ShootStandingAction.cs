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
    [SerializeField]
    private float shootInterval = 0.5f;
    private float timeSinceLastShoot;

    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);

        gunController = agentGameObj.GetComponentInChildren<GunController>();
        charController = agentGameObj.GetComponent<CharacterController>();
        enemySensor = agentGameObj.GetComponent<EnemyVisualSensor>();
    }

    public override void Activate() {

        if(enemySensor.VisibleEnemy == null) {
            Terminate();
            return;
        }

        target = enemySensor.VisibleEnemy.transform;
        timeSinceLastShoot = shootInterval;
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


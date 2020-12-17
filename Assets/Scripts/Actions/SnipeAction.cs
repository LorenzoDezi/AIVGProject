using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SnipeAction", menuName = "GOAP/Actions/SnipeAction")]
public class SnipeAction : GOAP.Action {

    private CharacterController charController;
    private EnemyVisualSensor enemyVisualSensor;
    private SniperController sniperController;
    private Transform targetTransf;

    [SerializeField, Tooltip("Sniper aim speed (slower)")]
    private float sniperAimSpeedDegrees = 50f;
    private float originalAimSpeed;

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);

        sniperController = agentGameObj.GetComponentInChildren<SniperController>();
        charController = agentGameObj.GetComponent<CharacterController>();
        enemyVisualSensor = agentGameObj.GetComponent<EnemyVisualSensor>();
    }

    public override bool Activate() {

        if (!enemyVisualSensor.EnemySpotted)
            return false;

        targetTransf = enemyVisualSensor.VisibleEnemy.transform;
        originalAimSpeed = charController.AimSpeedDegrees;
        charController.AimSpeedDegrees = sniperAimSpeedDegrees;
        sniperController.IsShooting = true;
        return true;
    }

    public override void Deactivate() {
        charController.AimSpeedDegrees = originalAimSpeed;
        sniperController.IsShooting = false;
    }

    public override void Update() {
        charController.AimAt(targetTransf.position);
    }
}

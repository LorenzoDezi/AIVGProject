using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SnipeAction", menuName = "GOAP/Actions/SnipeAction")]
public class SnipeAction : GOAP.Action {

    private CharacterController charController;
    private EnemyVisualSensor enemyVisualSensor;
    private SniperController sniperController;
    private Transform targetTransf;
    private Transform transform;

    [SerializeField, Tooltip("Sniper track speed (slower)")]
    private float sniperTrackSpeedDegrees = 50f;
    [SerializeField, Tooltip("Sniper aim speed (slower)")]
    private float sniperAimSpeedDegrees = 50f;
    [SerializeField, Tooltip("Angle treshold where there will be the change from track to aim speed")]
    private float trackToAimAngleTreshold = 10f;
    private float originalAimSpeed;

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);

        sniperController = agentGameObj.GetComponentInChildren<SniperController>();
        charController = agentGameObj.GetComponent<CharacterController>();
        enemyVisualSensor = agentGameObj.GetComponent<EnemyVisualSensor>();
        transform = agentGameObj.GetComponent<Transform>();
    }

    public override bool Activate() {

        if (!enemyVisualSensor.EnemySpotted)
            return false;

        targetTransf = enemyVisualSensor.VisibleEnemy.transform;
        enemyVisualSensor.CurrWeaponRange = sniperController.WeaponRange;
        originalAimSpeed = charController.AimSpeedDegrees;
        sniperController.SetUsing(true);
        return true;
    }

    public override void Deactivate() {
        charController.AimSpeedDegrees = originalAimSpeed;
        sniperController.SetUsing(false);
    }

    public override void Update() {

        float angleTowardTarget = Mathf.Abs(Vector3.Angle(
            transform.right, (targetTransf.position - transform.position).normalized
        ));
        bool canShoot = angleTowardTarget <= trackToAimAngleTreshold;

        charController.AimSpeedDegrees =  canShoot ? 
            sniperAimSpeedDegrees : sniperTrackSpeedDegrees;
        sniperController.IsShooting = canShoot;

        charController.AimAt(targetTransf.position);
    }
}

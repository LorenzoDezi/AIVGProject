using UnityEngine;

[CreateAssetMenu(fileName = "ShootAction", menuName = "GOAP/Actions/ShootStandingAction")]
public class ShootAction : GOAP.Action {

    protected VisualSensor visualSensor;
    protected GunController gunController;
    protected CharacterController charController;
    protected Transform transform;
    protected Transform target;
    [SerializeField]
    protected LayerMask obstacleLayerMask;

    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);

        gunController = agentGameObj.GetComponentInChildren<GunController>();
        charController = agentGameObj.GetComponent<CharacterController>();
        visualSensor = agentGameObj.GetComponent<VisualSensor>();
        transform = agentGameObj.GetComponent<Transform>();
    }

    public override bool Activate() {

        if(!visualSensor.IsEnemySpotted)
            return false;

        target = visualSensor.VisibleEnemy;
        return true;
    }

    public override void Deactivate() {
        target = null;
    }

    public override void Update() {
        charController.AimAt(target.position);
        if (!transform.HasObstacleInBetween(target, obstacleLayerMask))
            gunController.TryToShoot();
    }
}


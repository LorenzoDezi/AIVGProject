using UnityEngine;

[CreateAssetMenu(fileName = "ShootAction", menuName = "GOAP/Actions/ShootStandingAction")]
public class ShootAction : GOAP.Action {

    protected EnemyVisualSensor visualSensor;
    protected GunController gunController;
    protected CharacterController charController;
    protected Transform transform;
    protected Transform target;
    [SerializeField]
    protected LayerMask obstacleLayerMask;

    [SerializeField]
    protected float shootInterval = 0.5f;
    protected float timeSinceLastShoot;

    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);

        gunController = agentGameObj.GetComponentInChildren<GunController>();
        charController = agentGameObj.GetComponent<CharacterController>();
        visualSensor = agentGameObj.GetComponent<EnemyVisualSensor>();
        transform = agentGameObj.GetComponent<Transform>();
    }

    public override bool Activate() {

        if(!visualSensor.EnemySpotted)
            return false;

        target = visualSensor.VisibleEnemy.transform;
        timeSinceLastShoot = shootInterval;
        return true;
    }

    public override void Deactivate() {
        target = null;
    }

    public override void Update() {

        charController.AimAt(target.position);

        if (timeSinceLastShoot >= shootInterval && 
            !target.HasObstacleInBetween(transform, obstacleLayerMask)) {

            gunController.TryToShoot();
            timeSinceLastShoot = 0f;

        } else
            timeSinceLastShoot += Time.deltaTime;
    }
}


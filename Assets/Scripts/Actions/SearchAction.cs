using UnityEngine;

public abstract class SearchAction : GOAP.Action {

    protected EnemyVisualSensor visualSensor;
    protected NavigationComponent navComponent;
    protected CharacterController charController;
    protected Transform transform;

    [SerializeField]
    protected float deltaLookRot = 10f;
    [SerializeField]
    protected float maxLookRot = 50f;
    [SerializeField]
    protected float minSearchSpeed = 2f;

    protected float currentLookRot;

    protected Vector2 lookDir;
    protected Vector2 startLookDir;
    protected Vector3 searchPoint;

    protected float startSqrDistance;
    protected float startSpeed;

    protected abstract void OnPathCompleted(bool success);
 
    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);

        visualSensor = agentGameObj.GetComponent<EnemyVisualSensor>();
        navComponent = agentGameObj.GetComponent<NavigationComponent>();
        charController = agentGameObj.GetComponent<CharacterController>();
        transform = agentGameObj.GetComponent<Transform>();
        startSpeed = charController.MovementSpeed;
    }


    public override bool Activate() {
        currentLookRot = 0f;
        navComponent.PathCompleted += OnPathCompleted;
        return true;
    }

    protected void SetSearchPoint(Vector3 searchPoint) {
        this.searchPoint = searchPoint;
        startSqrDistance = transform.SqrDistance(searchPoint);
        navComponent.MoveTo(searchPoint);
        startLookDir = (searchPoint - transform.position).normalized;
        charController.MovementSpeed = startSpeed;
    }

    public override void Deactivate() {
        charController.MovementSpeed = startSpeed;
        navComponent.PathCompleted -= OnPathCompleted;
    }

    public override void Update() {

        charController.MovementSpeed = Mathf.Lerp(
            minSearchSpeed, startSpeed, 
            transform.SqrDistance(searchPoint) / startSqrDistance
        );

        currentLookRot += deltaLookRot * Time.deltaTime;
        if(Mathf.Abs(currentLookRot) > maxLookRot) {
            currentLookRot = Mathf.Sign(currentLookRot) * maxLookRot;
            deltaLookRot = -deltaLookRot;
        }

        lookDir = startLookDir.RotatedBy(currentLookRot);

        charController.AimAt((Vector2)transform.position + lookDir);
    }
}


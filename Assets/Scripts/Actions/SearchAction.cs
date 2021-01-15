using UnityEngine;

[CreateAssetMenu(fileName = "SearchAction", menuName = "GOAP/Actions/SearchAction")]
public class SearchAction : GOAP.Action {

    private EnemyVisualSensor visualSensor;
    private NavigationComponent navComponent;
    private CharacterController charController;
    private Transform transform;

    [SerializeField]
    private int maxSearchPointIterations = 5;
    [SerializeField]
    private float searchDistance = 5f;

    [SerializeField]
    private float deltaLookRot = 10f;
    [SerializeField]
    private float maxLookRot = 50f;
    [SerializeField]
    private float minSearchSpeed = 2f;
    [SerializeField]
    private float changeSearchPointThreshold = 4f;

    private float currentLookRot;

    private Vector2 lookDir;
    private Vector2 startLookDir;
    private Vector3 searchPoint;

    private float startSqrDistance;
    private float startSpeed;

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);

        visualSensor = agentGameObj.GetComponent<EnemyVisualSensor>();
        navComponent = agentGameObj.GetComponent<NavigationComponent>();
        charController = agentGameObj.GetComponent<CharacterController>();
        transform = agentGameObj.GetComponent<Transform>();
        startSpeed = charController.MovementSpeed;
    }


    public override bool Activate() {
        SetSearchPoint(visualSensor.LastSeenPosition);
        currentLookRot = 0f;
        navComponent.PathCompleted += OnPathCompleted;
        return true;
    }

    private void SetSearchPoint(Vector3 searchPoint) {
        this.searchPoint = searchPoint;
        startSqrDistance = transform.SqrDistance(searchPoint);
        navComponent.MoveTo(searchPoint);
        startLookDir = (searchPoint - transform.position).normalized;
        charController.MovementSpeed = startSpeed;
    }

    private void OnPathCompleted(bool success) {

        if (!success)
            return;

        for(int i = 0; i < maxSearchPointIterations; i++) {
            Vector2 direction = ((Vector2)visualSensor.LastSeenDirection).RotatedBy(Random.Range(-90f, 90f));
            Vector2 possibleSearchPoint = (Vector2) transform.position + direction * searchDistance;
            var node = AstarPath.active.GetNearest(possibleSearchPoint).node;
            if (node.Walkable) {
                SetSearchPoint(possibleSearchPoint);
                return;
            }
        }
    }

    public override void Deactivate() {
        charController.MovementSpeed = startSpeed;
        navComponent.PathCompleted -= OnPathCompleted;
    }

    public override void Update() {

        if((visualSensor.LastSeenPosition - searchPoint).sqrMagnitude 
            > changeSearchPointThreshold) {
            SetSearchPoint(visualSensor.LastSeenPosition);
        }

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


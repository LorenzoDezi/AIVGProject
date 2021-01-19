using UnityEngine;

[CreateAssetMenu(fileName = "OutOfDangerAction", menuName = "GOAP/Actions/OutOfDangerAction")]
public class OutOfDangerAction : GOAP.Action {

    private NavigationComponent navComp;
    private CharacterController charController;
    private DangerSensor dangerSensor;
    private Transform transform;

    private Collider2D[] results;

    [SerializeField]
    private int maxSearchOutOfDangerIterations = 5;
    [SerializeField]
    private int maxSearchHideResults = 10;

    [SerializeField]
    private LayerMask obstacleMask;

    [SerializeField]
    private float safeDistance = 2f;

    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);
        navComp = agentGameObj.GetComponent<NavigationComponent>();
        dangerSensor = agentGameObj.GetComponent<DangerSensor>();
        charController = agentGameObj.GetComponent<CharacterController>();
        transform = agentGameObj.transform;

        results = new Collider2D[maxSearchHideResults];
    }

    public override bool Activate() {
        Vector3? destination = GetDestination();
        if(destination.HasValue) {
            navComp.MoveTo(destination.Value);
            navComp.PathCompleted += OnPathCompleted;
            dangerSensor.DangerFound += OnNewDangerFound;
            return true;
        }
        return false;
    }

    private void OnNewDangerFound(IDangerous danger) {
        Vector3? destination = GetDestination();
        if (destination.HasValue)
            navComp.MoveTo(destination.Value);
    }

    private Vector3? GetDestination() {
        Vector3? destination = null;

        Vector2 dangerDirection = (transform.position - dangerSensor.DangerSource).normalized;
        Vector2 rotatedDangerDirection = Vector2.zero;

        for (int i = 0; i < maxSearchOutOfDangerIterations; i++) {

            rotatedDangerDirection = dangerDirection.RotatedBy(Random.Range(-90f, 90f));
            destination = dangerSensor.DangerSource +
                ((Vector3) rotatedDangerDirection) * (dangerSensor.DangerRadius + safeDistance);

            var node = AstarPath.active.GetNearest(destination.Value).node;
            if (node.Walkable)
                break;
            else
                destination = null;
        }
        return destination;
    }

    private void OnPathCompleted(bool isCompleted) {
        if(isCompleted) {
            dangerSensor.SetInDanger(false);
            Terminate(true);
        } else {
            Vector3? destination = GetDestination();
            if (destination.HasValue)
                navComp.MoveTo(destination.Value);
            else
                Terminate(false);
        }
    }

    public override void Deactivate() {
        navComp.PathCompleted -= OnPathCompleted;
        dangerSensor.DangerFound -= OnNewDangerFound;
    }

    public override void Update() {
        charController.AimAt(transform.position + navComp.DirectionToWaypoint);
    }
}


using UnityEngine;

[CreateAssetMenu(fileName = "SingleSearchAction", menuName = "GOAP/Actions/SingleSearchAction")]
public class SingleSearchAction : SearchAction {


    [SerializeField]
    private int maxSearchPointIterations = 5;
    [SerializeField]
    private float searchDistance = 5f;
    [SerializeField]
    protected float changeSearchPointThreshold = 4f;

    public override bool Activate() {
        if (!base.Activate())
            return false;
        SetSearchPoint(visualSensor.LastSeenPosition);
        return true;
    }

    public override void Update() {

        if ((visualSensor.LastSeenPosition - searchPoint).sqrMagnitude
            > changeSearchPointThreshold) {
            SetSearchPoint(visualSensor.LastSeenPosition);
        }
        base.Update();
    }

    protected override void OnPathCompleted(bool success) {

        if (!success)
            return;

        for (int i = 0; i < maxSearchPointIterations; i++) {
            Vector2 direction = ((Vector2)visualSensor.LastSeenDirection).RotatedBy(Random.Range(-90f, 90f));
            Vector2 possibleSearchPoint = (Vector2)transform.position + direction * searchDistance;
            var node = AstarPath.active.GetNearest(possibleSearchPoint).node;
            if (node.Walkable) {
                SetSearchPoint(possibleSearchPoint);
                return;
            }
        }
    }

}


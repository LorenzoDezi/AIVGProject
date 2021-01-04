using GOAP;
using UnityEngine;

[CreateAssetMenu(fileName = "CoverAllyAction", menuName = "GOAP/Actions/CoverAllyAction")]
public class CoverAllyAction : CircleShootAction {

    [SerializeField]
    private WorldStateKey needCoverKey;
    private Transform allyToCover;

    [SerializeField]
    private float maxDistanceFromAlly = 10f;
    private float maxDistanceSqr;

    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);

        maxDistanceSqr = maxDistanceFromAlly * maxDistanceFromAlly;
    }

    public override bool CheckProceduralConditions() {
        var ally = World.WorldStates[needCoverKey]?.GameObjectValue;
        if (ally == null || ally.GetInstanceID() == transform.GetInstanceID())
            return false;
        allyToCover = ally.transform;
        return allyToCover.SqrDistance(transform) <= maxDistanceSqr;
    }

    protected override void SetMovementTargetPosition() {
        movementTargetPos = target.position + (allyToCover.position - target.position) / 2f;
    }

}


using GOAP;
using UnityEngine;

[CreateAssetMenu(fileName = "CoverAllyAction", menuName = "GOAP/Actions/CoverAllyAction")]
public class CoverAllyAction : CircleShootAction {

    [SerializeField]
    private WorldStateKey squadObjectKey;

    private SquadNeedCoverSensor needCoverSensor;
    WorldStates perception;
    private SquadComponent agentToCover;

    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);
        perception = agentGameObj.GetComponent<Agent>().WorldPerception;
    }

    public override bool Activate() {

        if (!base.Activate())
            return false;

        var squadGameObject = perception[squadObjectKey]?.GameObjectValue;
        if (!squadGameObject)
            return false;

        needCoverSensor = squadGameObject.GetComponent<SquadNeedCoverSensor>();
        agentToCover = needCoverSensor.GetMemberWhoNeedCover(transform.position);

        return agentToCover != null;
    }

    public override void Deactivate() {
        needCoverSensor.AddMemberWhoNeedCover(agentToCover);
    }

    protected override void SetMovementTargetPosition() {
        movementTargetPos = target.position + (agentToCover.Transform.position - target.position) / 2f;
    }

}


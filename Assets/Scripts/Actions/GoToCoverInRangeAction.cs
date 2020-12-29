using UnityEngine;

[CreateAssetMenu(fileName = "GoToCoverInRangeAction", menuName = "GOAP/Actions/GoToCoverInRangeAction")]
public class GoToCoverInRangeAction : GoToCoverAction {

    public override bool CheckProceduralConditions() {
        return target == null || GetTargetCover() != null;
    }

    protected override CoverComponent GetTargetCover() {
        return coverSensor.GetBestCoverInRange(visualSensor.CurrWeaponRange, target);
    }

}



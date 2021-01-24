using UnityEngine;

[CreateAssetMenu(fileName = "GoToCoverInRangeAction", menuName = "GOAP/Actions/GoToCoverInRangeAction")]
public class GoToCoverInRangeAction : GoToCoverAction {

    protected override CoverComponent GetTargetCover() {
        return coverSensor.GetBestCoverInRange(visualSensor.CurrWeaponRange, target);
    }

}



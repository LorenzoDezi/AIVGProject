using UnityEngine;

[CreateAssetMenu(fileName = "GoToCoverAction", menuName = "GOAP/Actions/GoToCoverAction")]
public class GoToCoverAction : ShootAction {

    protected CoverSensor coverSensor;
    protected CoverComponent targetCover;
    protected bool coverReached;


    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);

        coverSensor = agentGameObj.GetComponent<CoverSensor>();
    }

    public override bool CheckProceduralConditions() {
        return target == null || GetTargetCover() != null;
    }

    protected virtual CoverComponent GetTargetCover() {
        return coverSensor.GetBestCover();        
    }

    public override bool Activate() {

        if (!base.Activate()) {
            return false;
        }

        targetCover = GetTargetCover();
        if (targetCover == null)
            return false;

        MoveToCover();
        return true;
    }

    private void MoveToCover() {
        //Occupy the cover also if the agent is not there yet,
        //to avoid others to reach the same cover in the same time
        targetCover.IsOccupied = true;
        navComponent.PathCompleted += OnPathCompleted;
        coverReached = false;
        navComponent.MoveTo(targetCover.Transform.position);
    }

    public override void Deactivate() {
        base.Deactivate();
        if(!coverReached) {
            targetCover.IsOccupied = false;
            navComponent.PathCompleted -= OnPathCompleted;
        }
        navComponent.Stop();
    }

    protected void OnPathCompleted(bool success) {
        if (success) {
            coverSensor.EnterCover(targetCover);
            navComponent.PathCompleted -= OnPathCompleted;
            coverReached = true;
        }
        Terminate(success);
    }

}



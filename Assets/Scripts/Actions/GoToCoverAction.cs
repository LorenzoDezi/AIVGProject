using UnityEngine;

[CreateAssetMenu(fileName = "GoToCoverAction", menuName = "GOAP/Actions/GoToCoverAction")]
public class GoToCoverAction : ShootAction {

    protected CoverSensor coverSensor;
    protected NavigationComponent navComponent;
    protected CoverComponent targetCover;
    protected bool coverReached;


    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);

        coverSensor = agentGameObj.GetComponent<CoverSensor>();
        navComponent = agentGameObj.GetComponent<NavigationComponent>();
    }

    protected virtual CoverComponent GetTargetCover() {
        return coverSensor.GetBestCover();
    }

    public override bool Activate() {
        if(!base.Activate()) {
            return false;
        }
        
        coverReached = false;      
        targetCover = GetTargetCover();
        if (targetCover == null)
            return false;
        targetCover.IsOccupied = true;
        navComponent.PathCompleted += OnPathCompleted;
        navComponent.MoveTo(targetCover.Transform.position);
        return true;
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
            coverSensor.GoInCover(targetCover);
            navComponent.PathCompleted -= OnPathCompleted;
            coverReached = true;
        }
        Terminate(success);
    }

}



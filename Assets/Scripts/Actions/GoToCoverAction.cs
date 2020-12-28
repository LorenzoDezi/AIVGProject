using UnityEngine;

[CreateAssetMenu(fileName = "GoToCoverAction", menuName = "GOAP/Actions/GoToCoverAction")]
public class GoToCoverAction : ShootAction {

    private CoverSensor coverSensor;
    private NavigationComponent navComponent;
    private CoverComponent targetCover;
    private bool coverReached;


    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);

        coverSensor = agentGameObj.GetComponent<CoverSensor>();
        navComponent = agentGameObj.GetComponent<NavigationComponent>();
    }

    public override bool Activate() {
        if(!base.Activate()) {
            return false;
        }
        
        coverReached = false;
        targetCover = coverSensor.GetBestCover();
        if (targetCover == null)
            return false;
        targetCover.IsOccupied = true;
        navComponent.PathCompleted.AddListener(OnPathCompleted);
        navComponent.MoveTo(targetCover.Transform.position);
        return true;
    }

    public override void Deactivate() {
        base.Deactivate();
        if(!coverReached) {
            targetCover.IsOccupied = false;
            navComponent.PathCompleted.RemoveListener(OnPathCompleted);
        }
        navComponent.Stop();
    }

    private void OnPathCompleted(bool success) {
        if (success) {
            coverSensor.GoInCover(targetCover);
            navComponent.PathCompleted.RemoveListener(OnPathCompleted);
            coverReached = true;
        }
        Terminate(success);
    }

}



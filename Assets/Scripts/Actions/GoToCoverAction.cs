using UnityEngine;

[CreateAssetMenu(fileName = "GoToCoverAction", menuName = "GOAP/Actions/GoToCoverAction")]
public class GoToCoverAction : ShootAction {

    private CoverSensor coverSensor;
    private CoverComponent currBestCover;

    private NavigationComponent navComponent;


    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);

        coverSensor = agentGameObj.GetComponent<CoverSensor>();
        navComponent = agentGameObj.GetComponent<NavigationComponent>();
    }

    public override bool CheckProceduralConditions() {
        return coverSensor.BestCover != null;
    }

    public override bool Activate() {
        if(!base.Activate() || coverSensor.BestCover == null) {
            return false;
        }

        navComponent.PathCompleted.AddListener(OnPathCompleted);

        SetBestCover();
        coverSensor.BestCoverChanged.AddListener(SetBestCover);
        return true;
    }

    private void SetBestCover() {
        currBestCover = coverSensor.BestCover;
        if (currBestCover == null) {
            Terminate(false);
            return;
        }
        navComponent.MoveTo(currBestCover.Transform.position);
    }

    private void OnPathCompleted(bool success) {
        if(success)
            coverSensor.CurrCover = currBestCover;
        Terminate(success);
    }

    public override void Deactivate() {
        base.Deactivate();
        coverSensor.BestCoverChanged.RemoveListener(SetBestCover);
        navComponent.Stop();
    }
    
}



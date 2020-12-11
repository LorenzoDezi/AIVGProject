using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOAP;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolAction", menuName = "GOAP/Actions/PatrolAction")]
public class PatrolAction : GOAP.Action {
    //TODO Refactor patrol points, avoid EnemyGuardAgent - it breaks the logic
    public List<Transform> PatrolPoints { get; set; }
    private int currPatrolIndex;
    private CharacterController characterController;
    private NavigationComponent navigationComp;
    private Transform transform;

    [SerializeField]
    private WorldStateKey patrollingKey;
    private WorldState currPatrollingState;

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);
        navigationComp = agentGameObj.GetComponent<NavigationComponent>();
        transform = agentGameObj.transform;
        characterController = agentGameObj.GetComponent<CharacterController>();
        currPatrollingState = new WorldState(patrollingKey, false);
        agent.UpdatePerception(currPatrollingState);
    }

    public override bool Activate() {
        navigationComp.PathCompleted.AddListener(OnPathCompleted);
        SetPatrolToCloserPatrolPoint();
        SetPatrollingWorldState(true);
        return true;
    }


    public override void Update() {

    }

    private void SetPatrolToCloserPatrolPoint() {
        Transform closerPatrolPoint = null;
        float minSqrDistance = Mathf.Infinity;
        for (int i = 0; i < PatrolPoints.Count; i++) {
            float currSqrDistance = Vector3.SqrMagnitude(PatrolPoints[i].position - transform.position);
            if (currSqrDistance < minSqrDistance) {
                minSqrDistance = currSqrDistance;
                closerPatrolPoint = PatrolPoints[i];
                currPatrolIndex = i;
            }
        }
        SetPatrolTo(closerPatrolPoint);
    }

    public override void Deactivate() {
        navigationComp.Stop();
        navigationComp.PathCompleted.RemoveListener(OnPathCompleted);
        SetPatrollingWorldState(false);
    }

    private void SetPatrollingWorldState(bool value) {
        currPatrollingState.BoolValue = value;
        agent.UpdatePerception(currPatrollingState);
    }

    private void SetPatrolTo(Transform closerPatrolPoint) {
        characterController.AimAt(closerPatrolPoint.position);
        navigationComp.MoveTo(closerPatrolPoint.position);
    }

    private void OnPathCompleted(bool success) {
        currPatrolIndex++;
        if (currPatrolIndex >= PatrolPoints.Count)
            currPatrolIndex = 0;
        SetPatrolTo(PatrolPoints[currPatrolIndex]);
    } 
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOAP;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolAction", menuName = "GOAP/Actions/PatrolAction")]
public class PatrolAction : GOAP.Action {

    public List<Transform> PatrolPoints { get; set; }
    private int currPatrolIndex;
    private CharacterController characterController;
    private NavigationComponent navigationComp;
    private Transform transform;

    public override void Init(GameObject agentGameObj, GOAP.Action actionTemplate) {
        this.preconditions = new WorldStates(actionTemplate.Preconditions);
        this.effects = new WorldStates(actionTemplate.Effects);
        this.cost = actionTemplate.Cost;
        navigationComp = agentGameObj.GetComponent<NavigationComponent>();
        navigationComp.PathCompleted.AddListener(OnPathCompleted);
        transform = agentGameObj.transform;
        characterController = agentGameObj.GetComponent<CharacterController>();
    }

    public override void Activate() {
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
    }

    public override void Update() {
        
    }

    private void SetPatrolTo(Transform closerPatrolPoint) {
        characterController.AimAt(closerPatrolPoint.position);
        navigationComp.MoveTo(closerPatrolPoint);
    }

    private void OnPathCompleted() {
        currPatrolIndex++;
        if (currPatrolIndex >= PatrolPoints.Count)
            currPatrolIndex = 0;
        SetPatrolTo(PatrolPoints[currPatrolIndex]);
    }

    
}


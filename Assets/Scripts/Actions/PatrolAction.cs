﻿using System;
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

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);
        navigationComp = agentGameObj.GetComponent<NavigationComponent>();
        transform = agentGameObj.transform;
        characterController = agentGameObj.GetComponent<CharacterController>();
    }

    public override void Activate() {
        Debug.LogFormat("Activated called on action {0}", name);
        navigationComp.PathCompleted.AddListener(OnPathCompleted);
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
        Debug.LogFormat("Deactivated called on action {0}", name);
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


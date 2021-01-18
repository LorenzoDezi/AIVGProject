using System.Collections.Generic;
using UnityEngine;

public class PatrollerComponent : MonoBehaviour {

    [SerializeField]
    private List<Transform> patrolPoints;
    private int currPatrolIndex;

    private CharacterController characterController;
    private NavigationComponent navigationComp;
    private new Transform transform;

    private void Awake() {
        characterController = GetComponent<CharacterController>();
        navigationComp = GetComponent<NavigationComponent>();
        transform = GetComponent<Transform>();
    }

    public void StartPatrol() {
        navigationComp.PathCompleted += OnPathCompleted;
        SetPatrolToCloserPatrolPoint();
    }

    public void StopPatrol() {
        navigationComp.Stop();
        navigationComp.PathCompleted -= OnPathCompleted;
    }

    public void UpdateAim() {
        characterController.AimAt(transform.position + navigationComp.DirectionToWaypoint);
    }

    private void OnPathCompleted(bool success) {
        currPatrolIndex++;
        if (currPatrolIndex >= patrolPoints.Count)
            currPatrolIndex = 0;
        SetPatrolTo(patrolPoints[currPatrolIndex]);
    }

    private void SetPatrolToCloserPatrolPoint() {
        Transform closerPatrolPoint = null;
        float minSqrDistance = Mathf.Infinity;
        for (int i = 0; i < patrolPoints.Count; i++) {
            float currSqrDistance = Vector3.SqrMagnitude(patrolPoints[i].position - transform.position);
            if (currSqrDistance < minSqrDistance) {
                minSqrDistance = currSqrDistance;
                closerPatrolPoint = patrolPoints[i];
                currPatrolIndex = i;
            }
        }
        SetPatrolTo(closerPatrolPoint);
    }

    private void SetPatrolTo(Transform closerPatrolPoint) {
        navigationComp.MoveTo(closerPatrolPoint.position);
    }
}


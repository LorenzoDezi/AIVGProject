using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NavigationComponent : MonoBehaviour {

    private Transform targetPosition;
    private Seeker seeker;
    private CharacterController controller;
    private Path path;

    private float waypointDistTreshold = 1;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath;

    public UnityEvent PathCompleted;

    private void Awake() {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
    }

    public void MoveTo(Transform targetPosition) {
        this.targetPosition = targetPosition;
        seeker.StartPath(transform.position, targetPosition.position);
    }

    public void Stop() {
        path = null;
        controller.Move(Vector2.zero);
    }

    private void OnEnable() {
        seeker.pathCallback += OnPathCalculated;
    }

    private void OnDisable() {
        seeker.pathCallback -= OnPathCalculated;
    }

    private void OnPathCalculated(Path p) {
        Debug.Log("A path was calculated. Did it fail with an error? " + p.error);
        if (p.error)
            return;
        path = p;
        currentWaypoint = 0;
    }

    private void Update() {
        if (path == null)
            return;
        float sqrDistanceTreshold = waypointDistTreshold * waypointDistTreshold; //TODO Refactor
        float sqrDistanceToWaypoint = Vector3.SqrMagnitude(path.vectorPath[currentWaypoint] - transform.position);
        Debug.LogFormat("Sqr distance to waypoint {0}", sqrDistanceToWaypoint);
        while (sqrDistanceToWaypoint < sqrDistanceTreshold) {
            if (currentWaypoint + 1 < path.vectorPath.Count) {
                currentWaypoint++;
                sqrDistanceToWaypoint = Vector3.SqrMagnitude(path.vectorPath[currentWaypoint] - transform.position);
            } else {
                PathCompleted.Invoke();
                Stop();
                return;
            }
        }
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        controller.Move(dir);
    }
}

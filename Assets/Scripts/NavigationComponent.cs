﻿using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PathCompletedEvent : UnityEvent<bool> { }

public class NavigationComponent : MonoBehaviour {

    private Seeker seeker;
    private CharacterController controller;
    private Path path;

    private float waypointDistTreshold = 0.25f;
    private float waypointDistSqrTreshold;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath;

    [SerializeField]
    private float recalculatePathToTargetInterval = 0.5f;
    private Coroutine pathTargetCoroutine;
    private Transform target;
    public Transform Target {
        get => target;
        set {

            target = value;
            bool hadTarget = pathTargetCoroutine != null;

            if (value == null && hadTarget) {
                StopCoroutine(pathTargetCoroutine);
                pathTargetCoroutine = null;
            } else if (!hadTarget) {
                pathTargetCoroutine = StartCoroutine(RecalculatePathToTarget());
            }
        }
    }

    public PathCompletedEvent PathCompleted = new PathCompletedEvent();

    public void MoveTo(Vector3 position) {
        seeker.StartPath(transform.position, position);
    }

    public void Stop() {
        path = null;
        controller.Move(Vector2.zero);
    }

    private void Awake() {
        seeker = GetComponent<Seeker>();
        waypointDistSqrTreshold = waypointDistTreshold >= 1 ?  
            waypointDistTreshold * waypointDistTreshold : waypointDistTreshold;
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable() {
        seeker.pathCallback += OnPathCalculated;
    }

    private void OnDisable() {
        seeker.pathCallback -= OnPathCalculated;
    }

    private void Update() {
        if (path == null)
            return;
        float sqrDistanceToWaypoint = Vector3.SqrMagnitude(path.vectorPath[currentWaypoint] - transform.position);
        while (sqrDistanceToWaypoint < waypointDistSqrTreshold) {
            if (currentWaypoint + 1 < path.vectorPath.Count) {
                currentWaypoint++;
                sqrDistanceToWaypoint = Vector3.SqrMagnitude(path.vectorPath[currentWaypoint] - transform.position);
            } else {
                PathCompleted.Invoke(true);
                Stop();
                return;
            }
        }
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        controller.Move(dir);
    }

    private IEnumerator RecalculatePathToTarget() {

        var wait = new WaitForSeconds(recalculatePathToTargetInterval);

        while(true) {
            MoveTo(target.position);
            yield return wait;
        }
    }

    private void OnPathCalculated(Path p) {
        if (p.error) {
            Debug.LogErrorFormat("Error calculating path {0}", p.errorLog);
            PathCompleted.Invoke(false);
            return;
        }
        path = p;
        currentWaypoint = 0;
    }

    public void OnDeath() {
        Stop();
        Destroy(this);
    }  
}

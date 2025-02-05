﻿using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public delegate void SearchTerminatedHandler();

public class SearchCoordinator : SquadSensor {

    private Collider2D[] results;

    [SerializeField]
    private LayerMask obstacleMask;
    [SerializeField]
    private LayerMask wallLayerMask;

    [SerializeField]
    private float searchRadius = 10f;
    [SerializeField]
    private int maxSearchPoints = 5;
    [SerializeField]
    private int maxRandomSearchPointIterations = 5;

    [SerializeField]
    private float searchTime = 10f;
    private Coroutine searchTimer;

    public event SearchTerminatedHandler SearchTerminated;

    private List<Vector3> searchPoints;

    private void Awake() {
        searchPoints = new List<Vector3>();
        results = new Collider2D[maxSearchPoints];
    }

    public override void Init(SquadManager manager) {
        base.Init(manager);        
    }

    public void SetupSearchPoints(Vector3 lastEnemyPosition) {

        searchPoints.Clear();

        AddHidingSearchPoints(lastEnemyPosition);

        for (int i = searchPoints.Count; i < maxSearchPoints; i++) {
            Vector3? randomSearchPoint = GetRandomSearchPoint(lastEnemyPosition);
            if (randomSearchPoint.HasValue)
                searchPoints.Add(randomSearchPoint.Value);
        }
        searchTimer = StartCoroutine(StartSearchTimer());
    }

    public Vector3? GetSearchPoint(Vector3 from) {
        Vector3? result = null;
        float minSqrDist = Mathf.Infinity;
        foreach(var searchPoint in searchPoints) {
            float currSqrDist = Vector3.SqrMagnitude(from - searchPoint);
            if(currSqrDist < minSqrDist) {
                minSqrDist = currSqrDist;
                result = searchPoint;
            }
        }
        if (result.HasValue)
            searchPoints.Remove(result.Value);
        return result;
    }

    public void Add(Vector3 searchPoint) {
        searchPoints.Add(searchPoint);
    }

    public void StopTimer() {
        if(searchTimer != null) {
            StopCoroutine(searchTimer);
            searchTimer = null;
        }
    }

    private IEnumerator StartSearchTimer() {
        yield return new WaitForSeconds(searchTime);
        SearchTerminated?.Invoke();
    }

    private void AddHidingSearchPoints(Vector3 lastEnemyPosition) {

        int count = Physics2D.OverlapCircleNonAlloc(lastEnemyPosition, searchRadius, results, obstacleMask);
        float searchRadiusSqr = searchRadius * searchRadius;
        for(int i = 0; i < count; i++) {
            Collider2D result = results[i];

            CoverComponent[] covers = result.GetComponentsInChildren<CoverComponent>();
            foreach(var cover in covers) {
                if (cover.CanCoverFrom(lastEnemyPosition)) {
                    searchPoints.Add(cover.Transform.position);
                    return;                    
                }
            }
        }
    }

    private Vector3? GetRandomSearchPoint(Vector3 lastEnemyPosition) {

        int iteration = 0;
        while (iteration < maxRandomSearchPointIterations) {

            Vector3 point = lastEnemyPosition;
            Vector2 displacement = UnityEngine.Random.insideUnitCircle * searchRadius;
            point.x += displacement.x;
            point.y += displacement.y;

            if (Physics2D.Linecast(point, lastEnemyPosition, wallLayerMask))
                continue;

            var node = AstarPath.active.GetNearest(point).node;
            if (node.Walkable)
                return (Vector3)node.position;
            iteration++;
        }
        return null;
    }

}


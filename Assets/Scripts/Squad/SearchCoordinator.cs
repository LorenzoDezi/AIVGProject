using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SearchCoordinator : SquadSensor {

    [SerializeField]
    private WorldStateKey squadObjectKey;
    private WorldState squadObjectWS;

    private Collider2D[] results;
    [SerializeField]
    private LayerMask hideLayerMask;
    private Queue<Vector3> searchPoints;
    [SerializeField]
    private float searchRadius = 10f;
    [SerializeField]
    private int maxSearchPoints = 5;
    [SerializeField]
    private int maxRandomSearchPointIterations = 5;

    private void Awake() {
        searchPoints = new Queue<Vector3>();
        results = new Collider2D[maxSearchPoints];
        squadObjectWS = new WorldState(squadObjectKey, this.gameObject);
    }

    public override void Init(SquadManager manager) {
        base.Init(manager);
        foreach(var squadMember in squadMembers) {
            squadMember.UpdatePerception(squadObjectWS);
        }
    }

    protected override void OnAddMember(SquadComponent newMember) {
        if (!squadMembers.Contains(newMember)) {
            squadMembers.Add(newMember);
            newMember.UpdatePerception(squadObjectWS);
        }
    }

    public void SetupSearchPoints(Vector3 lastEnemyPosition) {

        searchPoints.Clear();
        searchPoints.Enqueue(lastEnemyPosition);

        AddHidingSearchPoints(lastEnemyPosition);

        for (int i = searchPoints.Count; i < maxSearchPoints; i++) {
            Vector3? randomSearchPoint = GetRandomSearchPoint(lastEnemyPosition);
            if (randomSearchPoint.HasValue)
                searchPoints.Enqueue(randomSearchPoint.Value);
        }

        //DEBUG
        foreach (var searchPoint in searchPoints)
            Debug.DrawLine(lastEnemyPosition, searchPoint, Color.red, 5f);
    }

    public Vector3? GetSearchPoint() {
        if(searchPoints.Count > 0)
            return searchPoints.Dequeue();
        return null;
    }

    private void AddHidingSearchPoints(Vector3 lastEnemyPosition) {

        int count = Physics2D.OverlapCircleNonAlloc(lastEnemyPosition, searchRadius, results, hideLayerMask);

        for(int i = 0; i < count; i++) {
            Collider2D result = results[i];
            Vector3 dir = (result.transform.position - lastEnemyPosition).normalized;
            var outerHit = Physics2D.Raycast(result.bounds.center, dir, 2.5f, hideLayerMask);
            if (!outerHit)
                continue;
            Vector3 searchPoint = outerHit.point;
            var nodeInfo = AstarPath.active.GetNearest(searchPoint).node;
            if (nodeInfo.Walkable)
                searchPoints.Enqueue((Vector3)nodeInfo.position);
        }
    }

    private Vector3? GetRandomSearchPoint(Vector3 lastEnemyPosition) {

        int iteration = 0;
        while (iteration < maxRandomSearchPointIterations) {
            Vector3 point = lastEnemyPosition;
            Vector2 displacement = UnityEngine.Random.insideUnitCircle * searchRadius;
            point.x += displacement.x;
            point.y += displacement.y;
            var node = AstarPath.active.GetNearest(point).node;
            if (node.Walkable)
                return (Vector3)node.position;
            iteration++;
        }
        return null;
    }

}


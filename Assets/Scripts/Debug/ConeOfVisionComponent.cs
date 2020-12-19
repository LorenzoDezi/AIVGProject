using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct ConeRayInfo {
    public bool hasHit;
    public Vector3 hitPoint;
    public float distance;
    public float angle;

    public ConeRayInfo(bool hasHit, Vector3 hitPoint, float distance, float angle) {
        this.hasHit = hasHit;
        this.hitPoint = hitPoint;
        this.distance = distance;
        this.angle = angle;
    }
}

public struct EdgeInfo {
    public Vector3 pointA;
    public Vector3 pointB;

    public EdgeInfo(Vector3 pointA, Vector3 pointB) {
        this.pointA = pointA;
        this.pointB = pointB;
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ConeOfVisionComponent : MonoBehaviour {

    #region Cone of vision parameters
    [SerializeField]
    private EnemyVisualSensor visualSensor;
    private float coneOfVisionLenght;
    private float coneOfVisionAngle;
    private LayerMask obstacleLayer;
    #endregion

    #region Mesh creation parameters
    [Header("Mesh Generation Parameters")]
    private Mesh mesh;
    [SerializeField]
    private int rayCount = 2;
    [SerializeField]
    private int edgeResolveIterations;
    [SerializeField]
    private float edgeDistanceTreshold;
    private MeshFilter meshFilter;
    #endregion

    #region Caching fields
    private new Transform transform;
    private ConeRayInfo rayInfo = new ConeRayInfo();
    private List<Vector3> hitPoints = new List<Vector3>();
    private EdgeInfo edgeInfo = new EdgeInfo();
    private Vector3 rotatedDir = new Vector3();
    #endregion

    public Vector3 GetDirectionFromAngle(float angleInDegrees) {
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad),
            Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    private void Awake() {
        transform = GetComponent<Transform>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        coneOfVisionAngle = visualSensor.VisionAngle;
        coneOfVisionLenght = visualSensor.VisionLenght;
        obstacleLayer = visualSensor.ObstacleLayerMask;
    }

    private void LateUpdate() {
        TraceRays();
        UpdateMesh();
    }

    private void SetConeRayInfo(Vector2 direction, float angle) {
        direction.RotatedBy(angle, ref rotatedDir);
        var hit = Physics2D.Raycast(transform.position, rotatedDir, coneOfVisionLenght, obstacleLayer);
        rayInfo.hasHit = hit.collider != null;
        rayInfo.angle = angle;
        if(rayInfo.hasHit) {
            rayInfo.hitPoint = transform.InverseTransformPoint(hit.point);
            rayInfo.distance = hit.distance;
        } else {
            rayInfo.hitPoint = transform.InverseTransformPoint(transform.position + rotatedDir * coneOfVisionLenght);
            rayInfo.distance = coneOfVisionLenght;
        }
    }

    private void SetEdgeInfo(ConeRayInfo minConeRayInfo, ConeRayInfo maxConeRayInfo, Vector2 direction) {
        float minAngle = minConeRayInfo.angle;
        float maxAngle = maxConeRayInfo.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;
        for (int i = 0; i < edgeResolveIterations; i++) {
            //Finding the ray in the middle
            float angle = (minAngle + maxAngle) / 2;
            SetConeRayInfo(direction, angle);
            //Checking for a treshold, if both ray hit something but they're different objects
            bool edgeDstThresholdExceeded = Mathf.Abs(minConeRayInfo.distance - rayInfo.distance) > edgeDistanceTreshold;
            //If the new ray has hit something, it's the new minimum
            if (rayInfo.hasHit == minConeRayInfo.hasHit && !edgeDstThresholdExceeded) {
                minAngle = angle;
                minPoint = rayInfo.hitPoint;
            }
            //Otherwise it's the new maximum
            else {
                maxAngle = angle;
                maxPoint = rayInfo.hitPoint;
            }
        }
        edgeInfo.pointA = minPoint;
        edgeInfo.pointB = maxPoint;
    }

    private void TraceRays() {
        float angle = coneOfVisionAngle / 2;
        float angleIncrease = coneOfVisionAngle / rayCount;
        hitPoints.Clear();
        var prevConeRayInfo = new ConeRayInfo();
        for (int i = 0; i <= rayCount; i++) {
            SetConeRayInfo(transform.right, angle);
            if (i > 0)
                AddEdgeWith(prevConeRayInfo);
            hitPoints.Add(rayInfo.hitPoint);
            prevConeRayInfo = rayInfo;
            angle -= angleIncrease;
        }
    }

    private void AddEdgeWith(ConeRayInfo prevRayInfo) {
        bool edgeDistanceTresholdExceeded = Mathf.Abs(prevRayInfo.distance - rayInfo.distance) > edgeDistanceTreshold;
        if (prevRayInfo.hasHit != rayInfo.hasHit || (prevRayInfo.hasHit && rayInfo.hasHit && edgeDistanceTresholdExceeded)) {
            SetEdgeInfo(prevRayInfo, rayInfo, transform.right);
            if (edgeInfo.pointA != Vector3.zero) hitPoints.Add(edgeInfo.pointA);
            if (edgeInfo.pointB != Vector3.zero) hitPoints.Add(edgeInfo.pointB);
        }
    }

    private void UpdateMesh() {
        int vertexCount = hitPoints.Count + 1; //+1 for the origin, as the vertices
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangleIndices = new int[(vertexCount - 2) * 3]; //triangles are vertexCount - 2, * 3 as the indices
        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++) {
            var vertex = hitPoints[i];
            vertices[i + 1] = vertex;
            if (i < vertexCount - 2) {
                triangleIndices[i * 3] = 0;
                triangleIndices[i * 3 + 1] = i + 1;
                triangleIndices[i * 3 + 2] = i + 2;
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangleIndices;
        mesh.RecalculateNormals();
    }
}
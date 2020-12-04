using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyVisualSensor : Sensor {
    [Header("Visible and obstacle layers")]
    [SerializeField]
    private LayerMask enemyLayerMask;
    [SerializeField]
    private LayerMask obstacleLayerMask;

    public LayerMask ObstacleLayerMask => obstacleLayerMask;
    [SerializeField]
    private float visionAngle = 90f;
    public float VisionAngle => visionAngle;
    [SerializeField]
    private float visionLenght = 8f;
    public float VisionLenght => visionLenght;
    [SerializeField]
    private float checkTargetDelay = 0.2f;

    private ContactFilter2D contactFilter = new ContactFilter2D();
    private new Transform transform;
    private Vector2 enemyScaleOffset = new Vector2();
    private Collider2D[] results = new Collider2D[1];

    protected override void Awake() {
        base.Awake();
        transform = GetComponent<Transform>();
        currWorldStateTracked = new WorldState(keyToUpdate, null);
        contactFilter.SetLayerMask(enemyLayerMask);
        contactFilter.useTriggers = true;
    }

    private void Start() {
        StartCoroutine("CheckTargetWithDelay", checkTargetDelay);
    }

    IEnumerator CheckTargetWithDelay(float delay) {
        while (true) {
            SetEnemySeen(GetEnemyVisible());
            yield return new WaitForSeconds(delay);
        }
    }

    GameObject GetEnemyVisible() {
        GameObject enemy = null;
        if (Physics2D.OverlapCircle(transform.position, visionLenght,
            contactFilter, results) == 1) {
            var enemyCollider = results[0];
            Transform enemyTransform = enemyCollider.transform;
            Vector2 dirToPlayer = (enemyTransform.position - transform.position).normalized;
            if (Vector2.Angle(transform.right, dirToPlayer) <= visionAngle / 2f) {
                Vector2 position = enemyTransform.position;
                enemyScaleOffset.y = enemyTransform.localScale.y / 3f;
                if (CheckLineOfSight(position)) {
                    enemy = enemyCollider.gameObject;
                }
            }
        }
        return enemy;      
    }

    private void SetEnemySeen(GameObject enemy) {
        currWorldStateTracked.GameObjectValue = enemy;
        agentToUpdate.UpdatePerception(currWorldStateTracked);
    }

    private bool CheckLineOfSight(Vector2 colliderPosition) {
        var hit = Physics2D.Linecast(transform.position, colliderPosition, obstacleLayerMask);
        return hit.collider == null;
    }
}


using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyVisualSensor : MonoBehaviour {

    private Agent agentToUpdate;
    [SerializeField]
    private WorldStateKey enemySeenKey;
    private WorldState enemySeenWSTracked;

    [SerializeField]
    private WorldStateKey enemyNearKey;
    private WorldState enemyNearWSTracked;

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
    [SerializeField]
    private float sqrMinLoseSightDistance = 10f;
    [SerializeField]
    private float enemyNearTresholdDistance = 0.1f;
    private float currEnemyDistance;
    public Vector3 LastSeenPosition { get; private set; }
    public Vector3 LastSeenDirection { get; private set; }

    private ContactFilter2D contactFilter = new ContactFilter2D();
    private new Transform transform;
    private Collider2D[] results = new Collider2D[1];

    private bool enemySpotted;
    public bool EnemySpotted => enemySpotted;
    public GameObject VisibleEnemy { get; private set; }

    protected void Awake() {
        agentToUpdate = GetComponent<Agent>();
        transform = GetComponent<Transform>();
        enemySeenWSTracked = new WorldState(enemySeenKey, false);
        enemyNearWSTracked = new WorldState(enemyNearKey, false);
        contactFilter.SetLayerMask(enemyLayerMask);
        contactFilter.useTriggers = true;
    }

    private void Start() {
        StartCoroutine(CheckTargetWithDelay(checkTargetDelay));
    }

    IEnumerator CheckTargetWithDelay(float delay) {

        var wait = new WaitForSeconds(delay);
        Transform visibleEnemyTransf = null;

        while (true) { 

            if(enemySpotted) {
                CheckEnemyDistance(visibleEnemyTransf);
            } else {
                GameObject visibleEnemy = GetVisibleEnemy();
                if(visibleEnemy != null) {
                    VisibleEnemy = visibleEnemy;
                    UpdateEnemySeenWS(true);
                    visibleEnemyTransf = VisibleEnemy.transform;
                    enemySpotted = true;
                }
            }

            yield return wait;
        }
    }

    private void CheckEnemyDistance(Transform visibleEnemy) {

        currEnemyDistance = transform.SqrDistance(visibleEnemy);
        UpdateEnemyNearWS(currEnemyDistance < enemyNearTresholdDistance);

        if (currEnemyDistance > sqrMinLoseSightDistance) {
            VisibleEnemy = null;
            UpdateEnemySeenWS(false);
            enemySpotted = false;
        }
    }

    GameObject GetVisibleEnemy() {

        GameObject enemy = null;

        if (Physics2D.OverlapCircle(transform.position, visionLenght,
            contactFilter, results) == 1) {

            var enemyCollider = results[0];
            Transform enemyTransform = enemyCollider.transform;
            Vector2 dirToPlayer = (enemyTransform.position - transform.position).normalized;

            if (Vector2.Angle(transform.right, dirToPlayer) <= visionAngle / 2f &&
                !transform.HasObstacleInBetween(enemyTransform, obstacleLayerMask)) {

                enemy = enemyCollider.gameObject;

                if (enemy != null) {
                    LastSeenPosition = enemyTransform.position;
                    LastSeenDirection = enemyTransform.right;
                }
            }
        }

        return enemy;      
    }

    private void UpdateEnemySeenWS(bool value) {
        enemySeenWSTracked.BoolValue = value;
        agentToUpdate.UpdatePerception(enemySeenWSTracked);
    }

    private void UpdateEnemyNearWS(bool value) {
        enemyNearWSTracked.BoolValue = value;
        agentToUpdate.UpdatePerception(enemyNearWSTracked);
    }
}


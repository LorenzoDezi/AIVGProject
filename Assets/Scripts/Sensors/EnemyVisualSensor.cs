using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpottedEvent : UnityEvent<GameObject> { }

public class EnemyVisualSensor : MonoBehaviour {

    #region WorldStates
    [SerializeField]
    private WorldStateKey enemySeenKey;
    private WorldState enemySeenWSTracked;

    [SerializeField]
    private WorldStateKey enemyNearKey;
    private WorldState enemyNearWSTracked;

    [SerializeField]
    private WorldStateKey enemyInWeaponRangeKey;
    private WorldState enemyInWeaponRangeWSTracked;
    #endregion

    #region LayerMasks
    [Header("Visible and obstacle layers")]
    [SerializeField]
    private LayerMask enemyLayerMask;
    [SerializeField]
    private LayerMask obstacleLayerMask;
    public LayerMask ObstacleLayerMask => obstacleLayerMask;
    #endregion

    #region vision cone
    [Header("Vision cone parameters")]
    [SerializeField]
    private float visionAngle = 90f;
    public float VisionAngle => visionAngle;
    [SerializeField]
    private float visionLenght = 8f;
    public float VisionLenght => visionLenght;
    [SerializeField]
    private float checkTargetDelay = 0.2f; 
    #endregion

    #region enemy distance
    [Header("Sight and enemy distance parameters")]
    [SerializeField]
    private float sqrMinLoseSightDistance = 10f;
    [SerializeField]
    private float enemyNearTresholdDistance = 0.1f;
    private float currEnemyDistance;

    private float currWeaponRangeSqr;
    private float currWeaponRange;
    public float CurrWeaponRange {
        get => currWeaponRange;
        set {
            currWeaponRange = value;
            currWeaponRangeSqr = currWeaponRange * currWeaponRange;
        }
    }
    #endregion

    #region enemy visibility
    private bool enemySpotted;
    public bool EnemySpotted => enemySpotted;
    private Transform visibleEnemyTransform;
    public GameObject VisibleEnemy { get; private set; }

    public Vector3 LastSeenPosition { get; private set; }
    public Vector3 LastSeenDirection { get; private set; }
    public EnemySpottedEvent EnemySpottedEvent { get; } = new EnemySpottedEvent();
    #endregion

    #region cached fields
    private new Transform transform;
    private Agent agentToUpdate;
    private HealthComponent healthComp;
    private Collider2D[] cachedCheckResults = new Collider2D[1];
    #endregion

    protected void Awake() {

        agentToUpdate = GetComponent<Agent>();
        healthComp = GetComponent<HealthComponent>();
        healthComp.HealthChange.AddListener(OnEnemyAttack);
        transform = GetComponent<Transform>();

        enemySeenWSTracked = new WorldState(enemySeenKey, false);
        enemyNearWSTracked = new WorldState(enemyNearKey, false);
        enemyInWeaponRangeWSTracked = new WorldState(enemyInWeaponRangeKey, true);
        agentToUpdate.UpdatePerception(enemySeenWSTracked);
        agentToUpdate.UpdatePerception(enemyNearWSTracked);
        agentToUpdate.UpdatePerception(enemyInWeaponRangeWSTracked);

        CurrWeaponRange = Mathf.Infinity;
    }

    private void Start() {
        StartCoroutine(CheckTargetWithDelay(checkTargetDelay));
    }

    IEnumerator CheckTargetWithDelay(float delay) {

        var wait = new WaitForSeconds(delay);
        visibleEnemyTransform = null;

        while (true) { 

            if(enemySpotted) {

                UpdateEnemyDistance();

                if (currEnemyDistance > sqrMinLoseSightDistance) {
                    VisibleEnemy = null;
                    UpdateEnemySeenWS(false);
                    enemySpotted = false;
                }

            } else {
                GameObject visibleEnemy = GetVisibleEnemy();
                if(visibleEnemy != null) {
                    SpotEnemy(visibleEnemy);
                    EnemySpottedEvent.Invoke(visibleEnemy);
                }
            }

            yield return wait;
        }
    }

    private void UpdateEnemyDistance() {

        currEnemyDistance = transform.SqrDistance(visibleEnemyTransform);
        UpdateEnemyNearWS(currEnemyDistance < enemyNearTresholdDistance);
        UpdateEnemyInWeaponRangeWS(currEnemyDistance <= currWeaponRangeSqr);
    }

    private void OnEnemyAttack(float currHealth) {
        if(!enemySpotted && currHealth < healthComp.MaxHealth) {
            //TODO: A little weak... maybe find some other way if there is time
            EnemySpottedEvent.Invoke(GameManager.Player);
            SpotEnemy(GameManager.Player);
        }
    }

    public void SpotEnemy(GameObject visibleEnemy) {
        VisibleEnemy = visibleEnemy;
        UpdateEnemySeenWS(true);
        visibleEnemyTransform = VisibleEnemy.transform;
        enemySpotted = true;
    }

    GameObject GetVisibleEnemy() {

        GameObject enemy = null;

        if (Physics2D.OverlapCircleNonAlloc(transform.position, visionLenght,
            cachedCheckResults, enemyLayerMask) == 1) {

            var enemyCollider = cachedCheckResults[0];
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

    #region update WorldStates methods
    private void UpdateEnemySeenWS(bool value) {
        enemySeenWSTracked.BoolValue = value;
        agentToUpdate.UpdatePerception(enemySeenWSTracked);
    }

    private void UpdateEnemyNearWS(bool value) {
        enemyNearWSTracked.BoolValue = value;
        agentToUpdate.UpdatePerception(enemyNearWSTracked);
    }

    private void UpdateEnemyInWeaponRangeWS(bool value) {
        enemyInWeaponRangeWSTracked.BoolValue = value;
        agentToUpdate.UpdatePerception(enemyInWeaponRangeWSTracked);
    }
    #endregion
}


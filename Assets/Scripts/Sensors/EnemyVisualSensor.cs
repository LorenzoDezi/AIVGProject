using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpottedEvent : UnityEvent<Transform> { }

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
    private Transform visibleEnemy;
    public Transform VisibleEnemy => visibleEnemy;

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

    #region monobehaviour methods
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
    #endregion

    #region private methods
    private IEnumerator CheckTargetWithDelay(float delay) {

        var wait = new WaitForSeconds(delay);
        visibleEnemy = null;

        while (true) {

            if (enemySpotted) {

                UpdateEnemyDistance();

                if (currEnemyDistance > sqrMinLoseSightDistance) {
                    visibleEnemy = null;
                    UpdateEnemySeenWS(false);
                    enemySpotted = false;
                }

            } else {
                Transform visibleEnemy = GetVisibleEnemy();
                if (visibleEnemy != null) {
                    SpotEnemy(visibleEnemy);
                    EnemySpottedEvent.Invoke(visibleEnemy);
                }
            }

            yield return wait;
        }
    }

    private void UpdateEnemyDistance() {

        currEnemyDistance = transform.SqrDistance(visibleEnemy);
        UpdateEnemyNearWS(currEnemyDistance < enemyNearTresholdDistance);
        UpdateEnemyInWeaponRangeWS(currEnemyDistance <= currWeaponRangeSqr);
    }

    private void OnEnemyAttack(float currHealth) {
        if (!enemySpotted && currHealth < healthComp.MaxHealth) {
            //TODO: A little weak... maybe find some other way if there is time
            Transform enemy = GameManager.Player.transform;
            EnemySpottedEvent.Invoke(enemy);
            SpotEnemy(enemy);
        }
    }

    private Transform GetVisibleEnemy() {

        Transform visibleEnemyTransform = null;

        if (Physics2D.OverlapCircleNonAlloc(transform.position, visionLenght,
            cachedCheckResults, enemyLayerMask) == 1) {

            var enemyCollider = cachedCheckResults[0];
            Transform currEnemyTransform = enemyCollider.transform;
            Vector2 dirToPlayer = (currEnemyTransform.position - transform.position).normalized;

            if (Vector2.Angle(transform.right, dirToPlayer) <= visionAngle / 2f &&
                !transform.HasObstacleInBetween(currEnemyTransform, obstacleLayerMask)) {

                visibleEnemyTransform = currEnemyTransform;

                if (visibleEnemyTransform != null) {
                    LastSeenPosition = currEnemyTransform.position;
                    LastSeenDirection = currEnemyTransform.right;
                }
            }
        }

        return visibleEnemyTransform;
    }
    #endregion

    #region public methods
    public void SpotEnemy(Transform visibleEnemy) {
        this.visibleEnemy = visibleEnemy;
        UpdateEnemySeenWS(true);
        enemySpotted = true;
    } 
    #endregion

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


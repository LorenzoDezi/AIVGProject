using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public delegate void EnemySpottedHandler(Transform enemySpotted);
public delegate void EnemyLostHandler();

public class EnemyVisualSensor : MonoBehaviour {

    #region WorldStates
    [SerializeField]
    private WorldStateKey enemySeenKey;
    private WorldState enemySeenWSTracked;

    [SerializeField]
    private WorldStateKey enemyNearKey;
    private WorldState enemyNearWSTracked;

    [SerializeField]
    private WorldStateKey enemyLostKey;
    private WorldState enemyLostWSTracked;

    [SerializeField]
    private WorldStateKey enemyInWeaponRangeKey;
    private WorldState enemyInWeaponRangeWSTracked;
    #endregion

    #region LayerMasks
    [Header("Visible and obstacle layers")]
    [SerializeField]
    private LayerMask enemyLayerMask;
    [SerializeField]
    private LayerMask wallLayerMask;
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
    private bool isEnemySpotted;
    public bool IsEnemySpotted => isEnemySpotted;    
    private Transform visibleEnemy;
    private HealthComponent visibleEnemyHealth;
    public Transform VisibleEnemy => visibleEnemy;

    [SerializeField]
    private float searchTime = 3f;
    private Coroutine searchTimer;

    [SerializeField]
    private float timeToLoseEnemy = 5f;
    private float currTimeToLoseEnemy;

    public Vector3 LastSeenPosition { get; set; }
    public Vector3 LastSeenDirection { get; set; }
    public event EnemySpottedHandler EnemySpotted;
    public event EnemyLostHandler EnemyLost;
    #endregion

    #region cached fields
    private new Transform transform;
    private Agent agent;
    private HealthComponent healthComp;
    private Collider2D[] cachedCheckResults = new Collider2D[1];
    #endregion

    #region monobehaviour methods
    protected void Awake() {

        agent = GetComponent<Agent>();
        healthComp = GetComponent<HealthComponent>();
        transform = GetComponent<Transform>();

        InitPerception();

        CurrWeaponRange = Mathf.Infinity;
        healthComp.HealthChanged += OnEnemyAttack;
    }

    private void InitPerception() {
        enemySeenWSTracked = agent[enemySeenKey];
        if(enemySeenWSTracked == null) {
            enemySeenWSTracked = new WorldState(enemySeenKey, false);
            agent.Add(enemySeenWSTracked);
        }

        enemyNearWSTracked = agent[enemyNearKey];
        if(enemyNearWSTracked == null) {
            enemyNearWSTracked = new WorldState(enemyNearKey, false);
            agent.Add(enemyNearWSTracked);
        }

        enemyLostWSTracked = agent[enemyLostKey];
        if(enemyLostWSTracked == null) {
            enemyLostWSTracked = new WorldState(enemyLostKey, false);
            agent.Add(enemyLostWSTracked);
        }

        enemyInWeaponRangeWSTracked = agent[enemyInWeaponRangeKey];
        if(enemyInWeaponRangeWSTracked == null) {
            enemyInWeaponRangeWSTracked = new WorldState(enemyInWeaponRangeKey, true);
            agent.Add(enemyInWeaponRangeWSTracked);
        }
    }

    private void Start() {
        StartCoroutine(CheckTargetWithDelay(checkTargetDelay));
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (isEnemySpotted)
            return;
        var gameObj = collision.collider.gameObject;
        if(enemyLayerMask.ContainsLayer(gameObj.layer)) {
            Transform visibleEnemy = gameObj.transform;
            SpotEnemy(visibleEnemy);
            EnemySpotted?.Invoke(visibleEnemy);
        }       
    }
    #endregion


    #region public methods
    public void SpotEnemy(Transform visibleEnemy) {
        this.visibleEnemy = visibleEnemy;
        visibleEnemyHealth = visibleEnemy.GetComponent<HealthComponent>();
        visibleEnemyHealth.Death.AddListener(OnEnemyDeath);
        enemySeenWSTracked.BoolValue = true;
        StopSearch();
        enemyLostWSTracked.BoolValue = false;
        isEnemySpotted = true;
    }

    public void StopSearch() {
        if (searchTimer != null) {
            StopCoroutine(searchTimer);
            searchTimer = null;
        }
    }

    public GameObject CheckInConeOfVision(LayerMask goalLayerMask) {
        GameObject result = null;
        if (Physics2D.OverlapCircleNonAlloc(transform.position, visionLenght,
            cachedCheckResults, goalLayerMask) == 1) {
            Transform candidateObj = cachedCheckResults[0].transform;
            if (IsObjectVisible(candidateObj, obstacleLayerMask))
                result = candidateObj.gameObject;
        }
        return result;
    }
    #endregion

    #region private methods

    private void OnEnemyDeath() {
        visibleEnemy = null;
        enemyLostWSTracked.BoolValue = false;
        enemySeenWSTracked.BoolValue = false;
        isEnemySpotted = false;
    }

    private IEnumerator CheckTargetWithDelay(float delay) {

        var wait = new WaitForSeconds(delay);
        visibleEnemy = null;

        while (true) {

            if (isEnemySpotted) {
                UpdateEnemyDistance();
                CheckVisibility(delay);

            } else {
                Transform visibleEnemy = GetVisibleEnemy();
                if (visibleEnemy != null) {
                    SpotEnemy(visibleEnemy);
                    EnemySpotted?.Invoke(visibleEnemy);
                }
            }

            yield return wait;
        }
    }

    private void CheckVisibility(float timeSinceLastCheck) {
        if (!IsObjectVisible(visibleEnemy, wallLayerMask)) {
            if (currTimeToLoseEnemy > timeToLoseEnemy) {
                LoseEnemy();
                currTimeToLoseEnemy = 0f;
            } else
                currTimeToLoseEnemy += timeSinceLastCheck;

        } else {
            currTimeToLoseEnemy = 0f;
        }
    }

    private void LoseEnemy() {
        enemySeenWSTracked.BoolValue = false;
        enemyLostWSTracked.BoolValue = true;
        searchTimer = StartCoroutine(SearchTimer());

        LastSeenPosition = visibleEnemy.position;
        LastSeenDirection = visibleEnemy.right;
        visibleEnemy = null;
        visibleEnemyHealth.Death.RemoveListener(OnEnemyDeath);
        visibleEnemyHealth = null;
        isEnemySpotted = false;
        EnemyLost?.Invoke();
    }

    private IEnumerator SearchTimer() {
        yield return new WaitForSeconds(searchTime);
        enemyLostWSTracked.BoolValue = false;
    }

    private void UpdateEnemyDistance() {
        currEnemyDistance = transform.SqrDistance(visibleEnemy);
        enemyNearWSTracked.BoolValue = currEnemyDistance < enemyNearTresholdDistance;
        enemyInWeaponRangeWSTracked.BoolValue = currEnemyDistance <= currWeaponRangeSqr;
    }

    private void OnEnemyAttack(float currHealth) {
        if (!isEnemySpotted && currHealth < healthComp.MaxHealth) {
            //TODO: A little weak... maybe find some other way if there is time
            Transform enemy = GameManager.Player.transform;
            EnemySpotted?.Invoke(enemy);
            SpotEnemy(enemy);
        }
    }

    private Transform GetVisibleEnemy() {

        Transform visibleEnemy = null;

        if (Physics2D.OverlapCircleNonAlloc(transform.position, visionLenght,
            cachedCheckResults, enemyLayerMask) == 1) {

            var enemyCollider = cachedCheckResults[0];
            Transform candidateEnemy = enemyCollider.transform;
            if(IsObjectVisible(candidateEnemy, obstacleLayerMask))
                visibleEnemy = candidateEnemy;
        }

        return visibleEnemy;
    }  

    private bool IsObjectVisible(Transform obj, LayerMask obstacleLayerMask) {
        Vector2 dirToPlayer = (obj.position - transform.position).normalized;

        return Vector2.Angle(transform.right, dirToPlayer) <= visionAngle / 2f &&
            !transform.HasObstacleInBetween(obj, obstacleLayerMask);
    }
    #endregion

}


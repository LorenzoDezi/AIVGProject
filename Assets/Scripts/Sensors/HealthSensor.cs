using GOAP;
using System.Collections;
using UnityEngine;

public delegate void NearestHSChangedHandler();

public class HealthSensor : MonoBehaviour {

    private HealthComponent healthComp;
    private Agent agent;

    [Header("World State Keys")]
    [SerializeField]
    private WorldStateKey damageStateKey;
    private WorldState damageStateWSTracked;

    [SerializeField]
    private WorldStateKey stressLevelKey;
    private WorldState stressLevelWSTracked;

    [SerializeField]
    private WorldStateKey inCoverKey;
    private bool needCoverCheck;


    [Header("Stress parameters")]
    [SerializeField]
    private float stressDecreasePerSec = 0.2f;
    [SerializeField]
    private float stressDecreaseCoverPerSec = 0.5f;
    [SerializeField]
    private float stressIncreasePerLostHP = 0.1f;
    [SerializeField]
    private float maxStressLevel = 10f;

    [Header("HealthStation check parameters")]
    [SerializeField]
    private float checkForHealthStationsDelay = 2f;
    [SerializeField]
    private float checkForHealthStationsRadius = 5f;
    [SerializeField]
    private int checkHSMaxQueryResults = 5;
    [SerializeField]
    private LayerMask HSLayer;
    private Collider2D[] cachedCheckHSResults;
    private Coroutine checkHSRoutine;

    public HealthStation NearestHealthStation { get; private set; }
    public bool HasNearHealthStation { get; private set; }

    public event NearestHSChangedHandler NearestHSChanged;

    private float currHealth;

    private void Awake() {

        agent = GetComponent<Agent>();
        healthComp = GetComponent<HealthComponent>();

        InitPerception();

        currHealth = healthComp.MaxHealth;
        healthComp.HealthChanged += UpdatePerception;

        cachedCheckHSResults = new Collider2D[checkHSMaxQueryResults];
    }

    private void InitPerception() {
        damageStateWSTracked = agent[damageStateKey];
        if (damageStateWSTracked == null) {
            damageStateWSTracked = new WorldState(damageStateKey, 0);
            agent.Add(damageStateWSTracked);
        }

        stressLevelWSTracked = agent[stressLevelKey];
        if (stressLevelWSTracked == null) {
            stressLevelWSTracked = new WorldState(stressLevelKey, 0);
            agent.Add(stressLevelWSTracked);
        }
    }

    private void Start() {
        needCoverCheck = agent.WorldPerception[inCoverKey] != null;
    }

    private void Update() {

        if (stressLevelWSTracked.FloatValue > 0f)
            UpdateStressLevel();
    }

    private void UpdatePerception(float newCurrHealth) {

        float healthChange = newCurrHealth - this.currHealth;
        this.currHealth = newCurrHealth;

        ChangeStressLevelAfter(healthChange);

        UpdateDamageState();
    }

    private void ChangeStressLevelAfter(float healthChange) {

        float currStressLevel = stressLevelWSTracked.FloatValue;

        if (healthChange < 0 && currStressLevel < maxStressLevel)
            currStressLevel -= healthChange * stressIncreasePerLostHP; //More stress under fire
        else if (healthChange > 0)
            currStressLevel = 0f; //No stress if health points regained

        stressLevelWSTracked.FloatValue = currStressLevel;
    }

    private void UpdateDamageState() {
        damageStateWSTracked.FloatValue = healthComp.MaxHealth - this.currHealth;
        if (damageStateWSTracked.FloatValue > healthComp.MaxHealth / 2f) {
            if (checkHSRoutine == null)
                checkHSRoutine = StartCoroutine(CheckForHealthStations());
        } else if (checkHSRoutine != null) {
            StopCoroutine(checkHSRoutine);
            HasNearHealthStation = false;
            NearestHSChanged?.Invoke();
            checkHSRoutine = null;
        }
    }

    private void UpdateStressLevel() {
        float decreaseForSecond = stressDecreasePerSec;
        if (needCoverCheck && agent.WorldPerception[inCoverKey].BoolValue)
            decreaseForSecond = stressDecreaseCoverPerSec;

        float newStressValue = stressLevelWSTracked.FloatValue - decreaseForSecond * Time.deltaTime;
        if (newStressValue <= 0f)
            newStressValue = 0f;

        stressLevelWSTracked.FloatValue = newStressValue;
    }

    private IEnumerator CheckForHealthStations() {
        var wait = new WaitForSeconds(checkForHealthStationsDelay);
        while(true) {
            var foundHealthStation = GetBestHealthStation();
            HasNearHealthStation = foundHealthStation != null;
            if(HasNearHealthStation) {
                if (foundHealthStation != NearestHealthStation) {

                    if (HasNearHealthStation) {
                        NearestHealthStation = foundHealthStation;
                    }
                    NearestHSChanged?.Invoke();
                }
            } else {
                NearestHSChanged?.Invoke();
            }            
            yield return wait;
        }       
    }

    private HealthStation GetBestHealthStation() {

        int resultCount = Physics2D.OverlapCircleNonAlloc(
            transform.position, checkForHealthStationsRadius,
            cachedCheckHSResults, HSLayer
        );

        float sqrMinDistance = float.PositiveInfinity;
        HealthStation result = null;
        for (int i = 0; i < resultCount && i < checkHSMaxQueryResults; i++) {

            HealthStation current = cachedCheckHSResults[i].GetComponent<HealthStation>();
            if (current == null ||
                !current.CanRefill)
                continue;

            float currSqrDist = Vector3.SqrMagnitude(
                cachedCheckHSResults[i].transform.position - transform.position
            );
            if (currSqrDist < sqrMinDistance) {
                result = current;
                sqrMinDistance = currSqrDist;
            }
        }
        return result;
    }

       

    
}


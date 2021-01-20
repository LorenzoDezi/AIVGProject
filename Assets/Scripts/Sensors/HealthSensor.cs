using GOAP;
using UnityEngine;

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


    private float currHealth;

    private void Awake() {

        agent = GetComponent<Agent>();
        healthComp = GetComponent<HealthComponent>();

        InitPerception();

        currHealth = healthComp.MaxHealth;
        healthComp.HealthChanged += UpdatePerception;
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

    private void UpdatePerception(float newCurrHealth) {

        float healthChange = newCurrHealth - this.currHealth;
        this.currHealth = newCurrHealth;

        ChangeStressLevelAfter(healthChange);

        damageStateWSTracked.FloatValue = healthComp.MaxHealth - this.currHealth;
    }

    private void Start() {
        needCoverCheck = agent.WorldPerception[inCoverKey] != null;
    }

    private void Update() {

        if (stressLevelWSTracked.FloatValue > 0f)
            UpdateStressLevel();
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

    private void ChangeStressLevelAfter(float healthChange) {

        float currStressLevel = stressLevelWSTracked.FloatValue;

        if (healthChange < 0 && currStressLevel < maxStressLevel)
            currStressLevel -= healthChange * stressIncreasePerLostHP; //More stress under fire
        else if (healthChange > 0)
            currStressLevel = 0f; //No stress if health points regained

        stressLevelWSTracked.FloatValue = currStressLevel;
    }
}


using GOAP;
using UnityEngine;

public class HealthSensor : MonoBehaviour {

    private HealthComponent healthComp;
    private Agent agentToUpdate;

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
    private float stressLevelDecreaseForSecond = 0.2f;
    [SerializeField]
    private float stressLevelDecreaseInCover = 0.5f;
    [SerializeField]
    private float stressLevelIncreasePerHealthPoint = 0.1f;
    [SerializeField]
    private float maxStressLevel = 10f;


    private float lastHealthRegistered;

    private void Awake() {

        agentToUpdate = GetComponent<Agent>();
        healthComp = GetComponent<HealthComponent>();

        damageStateWSTracked = new WorldState(damageStateKey, 0f);
        agentToUpdate.UpdatePerception(damageStateWSTracked);
        stressLevelWSTracked = new WorldState(stressLevelKey, 0);
        agentToUpdate.UpdatePerception(stressLevelWSTracked);

        lastHealthRegistered = healthComp.MaxHealth;
        healthComp.HealthChanged += UpdatePerception;    
    }

    private void Start() {
        needCoverCheck = agentToUpdate.WorldPerception[inCoverKey] != null;
    }

    private void Update() {

        if (stressLevelWSTracked.FloatValue <= 0f)
            return;

        float decreaseForSecond = stressLevelDecreaseForSecond;
        if (needCoverCheck && agentToUpdate.WorldPerception[inCoverKey].BoolValue)
            decreaseForSecond = stressLevelDecreaseInCover;

        stressLevelWSTracked.FloatValue -= decreaseForSecond * Time.deltaTime;
        if (stressLevelWSTracked.FloatValue <= 0f)
            stressLevelWSTracked.FloatValue = 0f;

        agentToUpdate.UpdatePerception(stressLevelWSTracked);
    }

    private void UpdatePerception(float currHealth) {

        float healthDecrease = lastHealthRegistered - currHealth;
        lastHealthRegistered = currHealth;

        if (healthDecrease > 0 && stressLevelWSTracked.FloatValue < maxStressLevel)
            stressLevelWSTracked.FloatValue += healthDecrease * stressLevelIncreasePerHealthPoint;
        else if (healthDecrease < 0) {
            stressLevelWSTracked.FloatValue = 0f;
        }
        agentToUpdate.UpdatePerception(stressLevelWSTracked);

        damageStateWSTracked.FloatValue = healthComp.MaxHealth - currHealth;
        agentToUpdate.UpdatePerception(damageStateWSTracked);
    }
}


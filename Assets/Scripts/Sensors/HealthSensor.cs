using GOAP;
using UnityEngine;

public class HealthSensor : MonoBehaviour {

    private HealthComponent healthComp;
    private Agent agentToUpdate;

    [SerializeField]
    private WorldStateKey healthFullKey;
    private WorldState healthFullWSTracked;

    [SerializeField]
    private WorldStateKey stressLevelKey;
    private WorldState stressLevelWSTracked;

    private float lastHealthRegistered;

    protected void Awake() {

        agentToUpdate = GetComponent<Agent>();

        healthFullWSTracked = new WorldState(healthFullKey, true);
        agentToUpdate.UpdatePerception(healthFullWSTracked);
        stressLevelWSTracked = new WorldState(stressLevelKey, 0);
        agentToUpdate.UpdatePerception(stressLevelWSTracked);

        healthComp = GetComponent<HealthComponent>();
        lastHealthRegistered = healthComp.MaxHealth;
        healthComp?.HealthChange.AddListener(UpdatePerception);    
    }

    private void UpdatePerception(float currHealth) {

        if (currHealth < lastHealthRegistered)
            stressLevelWSTracked.IntValue += 1;
        else if (currHealth > lastHealthRegistered)
            stressLevelWSTracked.IntValue = 0;
        agentToUpdate.UpdatePerception(stressLevelWSTracked);

        healthFullWSTracked.BoolValue = healthComp.MaxHealth == currHealth;
        agentToUpdate.UpdatePerception(healthFullWSTracked);
    }
}


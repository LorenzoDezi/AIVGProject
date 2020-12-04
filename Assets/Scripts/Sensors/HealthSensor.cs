using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HealthSensor : Sensor {

    private HealthComponent healthComp;

    protected override void Awake() {
        base.Awake();
        currWorldStateTracked = new WorldState(keyToUpdate, true);
        agentToUpdate.UpdatePerception(currWorldStateTracked);
        healthComp = GetComponent<HealthComponent>();
        healthComp?.HealthChange.AddListener(UpdatePerception);
    }

    private void UpdatePerception(float currHealth) {
        currWorldStateTracked.BoolValue = healthComp.MaxHealth == currHealth;
        agentToUpdate.UpdatePerception(currWorldStateTracked);
    }
}


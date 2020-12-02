using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "StayAliveGoal", menuName = "GOAP/Goals/StayAliveGoal")]
public class StayAliveGoal : Goal {

    private float maxHealth;
    private float currHealth;

    [SerializeField, Tooltip("priority = (maxHealth - currHealth) * healthPriorityMultiplier")]
    private float healthPriorityMultiplier = 0.1f;

    public override void Init(GameObject agentObj) {
        base.Init(agentObj);
        var healthComponent = agentObj.GetComponent<HealthComponent>();
        maxHealth = healthComponent.MaxHealth;
        priority = 0f;
        healthComponent.HealthChange.AddListener(OnHealthChange);
    }

    private void OnHealthChange(float currHealth) {
        this.currHealth = currHealth;
        UpdatePriority();
    }

    protected override void UpdatePriority() {
        priority = (maxHealth - currHealth) * healthPriorityMultiplier;
        base.UpdatePriority();
    }
}


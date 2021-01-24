using GOAP;
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Agent))]
public class EnemyDebug : MonoBehaviour
{
    [NonSerialized]
    public Agent agent;
    [NonSerialized]
    public VisualSensor visualSensor;
    [NonSerialized]
    public CoverSensor coverSensor;
    [NonSerialized]
    public HealthComponent healthComponent;
    [NonSerialized]
    public List<GOAP.Action> currentPlan = new List<GOAP.Action>();
    [NonSerialized]
    public bool isDead;

    void Awake()
    {
#if UNITY_EDITOR
        agent = GetComponent<Agent>();
        agent.PlanCompleted += (plan) => currentPlan = plan;
        visualSensor = GetComponent<VisualSensor>();
        coverSensor = GetComponent<CoverSensor>();
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.Death.AddListener(OnDeath);
#endif
    }

    private void OnDeath() {
        isDead = true;
    }

}

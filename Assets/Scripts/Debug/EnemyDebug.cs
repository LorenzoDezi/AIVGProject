﻿using GOAP;
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
    public EnemyVisualSensor visualSensor;
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
        agent = GetComponent<Agent>();
        agent.PlanCompleted.AddListener((plan) => currentPlan = plan);
        visualSensor = GetComponent<EnemyVisualSensor>();
        coverSensor = GetComponent<CoverSensor>();
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.Death.AddListener(OnDeath);
    }

    private void OnDeath() {
        isDead = true;
    }

}

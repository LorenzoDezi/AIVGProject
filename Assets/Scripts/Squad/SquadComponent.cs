﻿using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadComponent : MonoBehaviour
{
    private Agent agent;
    private HealthComponent healthComp;
    private EnemyVisualSensor enemySensor;

    private SquadGoal currentSquadGoal;
    public int SquadIndex { get; set; }

    public event EnemySpottedHandler EnemySpotted {
        add {
            enemySensor.EnemySpotted += value;
        }
        remove {
            enemySensor.EnemySpotted -= value;
        }
    }

    public delegate void SquadCompDeathHandler(int squadIndex);
    public event SquadCompDeathHandler SquadCompDeath;

    private void Awake() {

        agent = GetComponent<Agent>();
        enemySensor = GetComponent<EnemyVisualSensor>();
        healthComp = GetComponent<HealthComponent>();
        healthComp.Death.AddListener(OnDeath);
    }

    private void OnDeath() {
        ResetGoal();
        SquadCompDeath?.Invoke(SquadIndex);
    }

    public void Spotted(Transform enemySpotted) {
        enemySensor.SpotEnemy(enemySpotted);
    }

    public void AddGoalWith(SquadGoal goalTemplate) {
        currentSquadGoal = Instantiate(goalTemplate);
        agent.Add(currentSquadGoal);
    }

    public void ResetGoal() {

        if (currentSquadGoal == null)
            return;

        agent.Remove(currentSquadGoal);
        Destroy(currentSquadGoal);
        currentSquadGoal = null;
    }
}

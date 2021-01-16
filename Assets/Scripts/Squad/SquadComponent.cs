using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadComponent : MonoBehaviour
{
    private Agent agent;
    private HealthComponent healthComp;
    private EnemyVisualSensor enemySensor;

    private SquadGoal currentSquadGoal;
    public int SquadGoalIndex { get; set; }

    public event EnemySpottedHandler EnemySpotted {
        add {
            enemySensor.EnemySpotted += value;
        }
        remove {
            enemySensor.EnemySpotted -= value;
        }
    }

    public event EnemyLostHandler EnemyLost {
        add {
            enemySensor.EnemyLost += value;
        }
        remove {
            enemySensor.EnemyLost -= value;
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
        SquadCompDeath?.Invoke(SquadGoalIndex);
    }

    public void Spotted(Transform enemySpotted) {
        enemySensor.SpotEnemy(enemySpotted);
    }

    public void UpdatePerception(WorldState worldState) {
        agent.UpdatePerception(worldState);
    }

    public void UpdateLastSeen(Transform enemy) {
        enemySensor.LastSeenPosition = enemy.position;
        enemySensor.LastSeenDirection = enemy.right;
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

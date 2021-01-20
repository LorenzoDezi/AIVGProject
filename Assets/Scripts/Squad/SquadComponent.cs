using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void SquadCompDeathHandler(int squadIndex);

public class SquadComponent : MonoBehaviour
{
    private Agent agent;
    private HealthComponent healthComp;
    private EnemyVisualSensor visualSensor;
    private DangerSensor dangerSensor;
    public Transform Transform { get; private set; }

    private SquadGoal currentSquadGoal;

    public int SquadIndex { get; set; }

    #region events
    public event EnemySpottedHandler EnemySpotted {
        add {
            visualSensor.EnemySpotted += value;
        }
        remove {
            visualSensor.EnemySpotted -= value;
        }
    }

    public event EnemyLostHandler EnemyLost {
        add {
            visualSensor.EnemyLost += value;
        }
        remove {
            visualSensor.EnemyLost -= value;
        }
    }

    public event DangerFoundHandler DangerFound {
        add {
            dangerSensor.DangerFound += value;
        }
        remove {
            dangerSensor.DangerFound -= value;
        }
    }

    public event SquadCompDeathHandler SquadCompDeath; 
    #endregion

    private void Awake() {

        agent = GetComponent<Agent>();
        visualSensor = GetComponent<EnemyVisualSensor>();
        dangerSensor = GetComponent<DangerSensor>();
        healthComp = GetComponent<HealthComponent>();
        Transform = GetComponent<Transform>();
        healthComp.Death.AddListener(OnDeath);
    }

    private void OnDeath() {
        ResetGoal();
        SquadCompDeath?.Invoke(SquadIndex);
    }

    #region public methods
    public WorldState this[WorldStateKey key] {
        get => agent.WorldPerception[key];
    }

    public void SpotEnemy(Transform enemySpotted) {
        visualSensor.SpotEnemy(enemySpotted);
    }

    public void StopSearch() {
        visualSensor.StopSearch();
    }

    public void Add(WorldState worldState) {
        agent.Add(worldState);
    }

    public void RegisterDanger(IDangerous danger) {
        dangerSensor.RegisterDanger(danger);
    }

    public void UpdateLastSeen(Transform enemy) {
        visualSensor.LastSeenPosition = enemy.position;
        visualSensor.LastSeenDirection = enemy.right;
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
    #endregion
}

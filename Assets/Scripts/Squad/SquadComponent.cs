using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SquadComponentDeath : UnityEvent<int> { }

public class SquadComponent : MonoBehaviour
{
    private Agent agent;
    private HealthComponent healthComp;
    private EnemyVisualSensor enemySensor;

    private SquadGoal currentSquadGoal;
    public int SquadIndex { get; set; }

    public EnemySpottedEvent EnemySpottedEvent => enemySensor.EnemySpottedEvent;
    public SquadComponentDeath Death { get; } = new SquadComponentDeath();

    private void Awake() {

        agent = GetComponent<Agent>();
        enemySensor = GetComponent<EnemyVisualSensor>();

        healthComp = GetComponent<HealthComponent>();
        healthComp.Death.AddListener(OnDeath);
    }

    private void OnDeath() {
        ResetGoal();
        Death.Invoke(SquadIndex);
    }

    public void Spotted(GameObject enemySpotted) {
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

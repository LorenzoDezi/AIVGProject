using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadComponent : MonoBehaviour
{

    private EnemyVisualSensor enemySensor;
    public EnemySpottedEvent EnemySpottedEvent => enemySensor.EnemySpottedEvent;

    private void Awake() {
        enemySensor = GetComponent<EnemyVisualSensor>();
    }

    public void Spotted(GameObject enemySpotted) {
        enemySensor.SpotEnemy(enemySpotted);
    }
}

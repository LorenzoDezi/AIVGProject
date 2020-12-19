using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    [SerializeField]
    private List<SquadComponent> squadMembers;
    private GameObject enemy;

    private void Start() {
        foreach(var squadMember in squadMembers) {
            squadMember.EnemySpottedEvent.AddListener(OnEnemySpottedEvent);
        }
    }

    public void OnEnemySpottedEvent(GameObject enemy) {
        this.enemy = enemy;
        foreach(var squadMember in squadMembers) {
            squadMember.Spotted(enemy);
        }
    }


}

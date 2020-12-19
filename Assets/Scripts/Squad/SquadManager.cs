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



    //How to change the goal based on what? Ideas:
        // When a team member goes to restore his health, it can be covered by another guy with more health
        // When the team counts only one member, it can send the remaining member to another squad
        // Assign one enemy to flank the player
}

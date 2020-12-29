using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    [SerializeField]
    private List<SquadComponent> squadMembers;
    [SerializeField]
    private List<SquadGoal> squadGoals;
    private Transform enemy;

    private void Start() {

        squadGoals.Sort(SquadGoal.SquadComparer);

        for(int i = 0; i < squadMembers.Count; i++) {

            var squadMember = squadMembers[i];
            squadMember.EnemySpottedEvent.AddListener(OnEnemySpottedEvent);
            squadMember.SquadIndex = i;
            squadMember.Death.AddListener(OnSquadComponentDeath);

            if (i < squadGoals.Count)
                squadMember.AddGoalWith(squadGoals[i]);
        }
    }

    public void OnEnemySpottedEvent(Transform enemy) {
        this.enemy = enemy;
        foreach(var squadMember in squadMembers) {
            squadMember.Spotted(enemy);
        }
    }

    private void OnSquadComponentDeath(int squadCompIndex) {

        for(int i = squadCompIndex + 1; i < squadMembers.Count; i++) {

            SquadComponent squadMember = squadMembers[i];
            squadMember.SquadIndex = i - 1;

            squadMember.ResetGoal();
            if(squadMember.SquadIndex < squadGoals.Count)
                squadMember.AddGoalWith(squadGoals[squadMember.SquadIndex]);
        }

        squadMembers.RemoveAt(squadCompIndex);
    }


}

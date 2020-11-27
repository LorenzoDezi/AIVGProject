using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolGoal", menuName = "GOAP/Goals/PatrolGoal")]
public class PatrolGoal : Goal {

    [SerializeField]
    WorldStateKey enemySeen;
    [SerializeField]
    private float lowPriority;
    [SerializeField]
    private float highPriority;

    WorldStates agentWorldStates;

    public override void Init(GameObject agentObj) {
        agentWorldStates = agentObj.GetComponent<Agent>().WorldPerception;
    }

    public override void UpdatePriority() {
        WorldState enemySeenState = agentWorldStates[enemySeen];
        if (enemySeenState != null)
            priority = enemySeenState.BoolValue ? lowPriority : highPriority;
    }
}


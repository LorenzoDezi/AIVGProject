using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "BoolPriorityGoal", menuName = "GOAP/Goals/BoolPriorityGoal")]
public class BoolPriorityGoal : Goal {

    [SerializeField]
    WorldState BoolState;
    [SerializeField]
    private float priorityIfFalse;
    [SerializeField]
    private float priorityIfTrue;

    WorldStates agentWorldStates;

    public override void Init(GameObject agentObj) {

        base.Init(agentObj);

        agentWorldStates = agentObj.GetComponent<Agent>().WorldPerception;
        WorldState enemySeenState = agentWorldStates[BoolState.Key];
        if(enemySeenState == null) {
            enemySeenState = new WorldState(BoolState);
            agentWorldStates.Add(enemySeenState);
        }

        priority = enemySeenState.BoolValue ? priorityIfTrue : priorityIfFalse;
        enemySeenState.StateChangeEvent.AddListener(UpdatePriority);
    }

    protected override void UpdatePriority() {

        WorldState enemySeenState = agentWorldStates[BoolState.Key];
        if (enemySeenState != null)
            priority = enemySeenState.BoolValue ? priorityIfTrue : priorityIfFalse;

        base.UpdatePriority();
    }
}


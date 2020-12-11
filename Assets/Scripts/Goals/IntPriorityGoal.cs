using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class IntPriorityGoal : Goal {

    [SerializeField]
    WorldState IntState;
    [SerializeField, Tooltip("priority = state.IntValue * priorityMultiplier")]
    private float priorityMultiplier;

    WorldStates agentWorldStates;

    public override void Init(GameObject agentObj) {

        base.Init(agentObj);

        agentWorldStates = agentObj.GetComponent<Agent>().WorldPerception;
        WorldState currState = agentWorldStates[IntState.Key];
        if (currState == null) {
            currState = new WorldState(IntState);
            agentWorldStates.Add(currState);
        }

        priority = currState.IntValue * priorityMultiplier;
        currState.StateChangeEvent.AddListener(UpdatePriority);
    }

    protected override void UpdatePriority() {

        WorldState currState = agentWorldStates[IntState.Key];
        if (currState != null)
            priority = currState.IntValue * priorityMultiplier;

        base.UpdatePriority();
    }
}


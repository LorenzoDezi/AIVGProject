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
        WorldState currState = agentWorldStates[BoolState.Key];
        if(currState == null) {
            currState = new WorldState(BoolState);
            agentWorldStates.Add(currState);
        }

        priority = currState.BoolValue ? priorityIfTrue : priorityIfFalse;
        currState.StateChangeEvent.AddListener(UpdatePriority);
    }

    protected override void UpdatePriority() {

        WorldState currState = agentWorldStates[BoolState.Key];
        if (currState != null)
            priority = currState.BoolValue ? priorityIfTrue : priorityIfFalse;

        base.UpdatePriority();
    }
}


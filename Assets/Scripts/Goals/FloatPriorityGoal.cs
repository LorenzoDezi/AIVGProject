using GOAP;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatPriorityGoal", menuName = "GOAP/Goals/FloatPriorityGoal")]
public class FloatPriorityGoal : Goal {

    [SerializeField]
    WorldState FloatState;
    [SerializeField, Tooltip("priority = state.FloatValue * priorityMultiplier")]
    private float priorityMultiplier;

    WorldStates agentWorldStates;

    public override void Init(GameObject agentObj) {

        base.Init(agentObj);

        agentWorldStates = agentObj.GetComponent<Agent>().WorldPerception;
        WorldState currState = agentWorldStates[FloatState.Key];
        if (currState == null) {
            currState = new WorldState(FloatState);
            agentWorldStates.Add(currState);
        }

        priority = currState.FloatValue * priorityMultiplier;
        currState.StateChangeEvent.AddListener(UpdatePriority);
    }

    protected override void UpdatePriority() {

        WorldState currState = agentWorldStates[FloatState.Key];
        if (currState != null)
            priority = currState.FloatValue * priorityMultiplier;

        base.UpdatePriority();
    }
}


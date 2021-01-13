using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SquadBehaviour : ScriptableObject {

    [SerializeField]
    private List<SquadGoal> goalsToComplete;
    [SerializeField, Tooltip("The world states that need to be matched to trigger this behaviour")]
    private WorldStates triggerStates;   
    SquadManager manager;

    private List<SquadComponent> membersAssigned;

    private bool active;
    public bool Active => active;

    public void Init(SquadManager manager) {
        this.manager = manager;
        membersAssigned = new List<SquadComponent>();
        WorldStates perception = manager.SquadPerception;
        foreach(WorldState triggerState in triggerStates) {
            WorldState percState = perception[triggerState.Key];
            if (percState == null) {
                percState = triggerState;
                perception.Add(percState);
            }
            percState.StateChanged += OnStateChange;
        }
    }

    private void OnStateChange() {
        if (!active && manager.SquadPerception.Contains(triggerStates)) {
            StartBehaviour();
        } else if (active && !manager.SquadPerception.Contains(triggerStates)) {
            StopBehaviour();
        }
    }

    protected virtual void StartBehaviour() {
        var members = manager.GetMembers(goalsToComplete.Count);
        if(members.Count > 0) {
            active = true;
            membersAssigned.AddRange(members);
            for(int i = 0; i < goalsToComplete.Count; i++) {
                membersAssigned[i].AddGoalWith(goalsToComplete[i]);
            }
        }
    }

    protected virtual void StopBehaviour() {
        foreach(var member in membersAssigned) {
            member.ResetGoal();
            manager.AddMember(member);
        }
        membersAssigned.Clear();
    }
    

}


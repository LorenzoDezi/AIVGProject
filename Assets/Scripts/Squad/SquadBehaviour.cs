using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "SquadBehaviour", menuName = "Squad/SquadBehaviour")]
public class SquadBehaviour : ScriptableObject {

    [SerializeField]
    private List<SquadGoal> goalTemplates;
    [SerializeField, Tooltip("The world states that need to be matched to trigger this behaviour")]
    private WorldStates triggerStates;   
    SquadManager manager;

    private List<SquadComponent> membersAssigned;

    private bool active;
    public bool Active => active;

    public void Init(SquadManager manager) {
        this.manager = manager;
        active = false;
        membersAssigned = new List<SquadComponent>();
        WorldStates perception = manager.SquadPerception;
        foreach(WorldState triggerState in triggerStates) {
            WorldState percState = perception[triggerState.Key];
            if (percState == null) {
                percState = new WorldState(triggerState);
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
        var members = manager.GetMembers(goalTemplates.Count);
        if(members.Count > 0) {
            active = true;
            membersAssigned.AddRange(members);
            for(int i = 0; i < goalTemplates.Count; i++) {
                membersAssigned[i].AddGoalWith(goalTemplates[i]);
            }
        }
    }

    protected virtual void StopBehaviour() {
        active = false;
        foreach(var member in membersAssigned) {
            manager.AddMember(member);
        }
        membersAssigned.Clear();
    }

}


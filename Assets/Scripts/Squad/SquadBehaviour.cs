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
        if (manager.SquadPerception.Contains(triggerStates)) {
            StartBehaviour();
        } else {
            StopBehaviour();
        }
    }

    private void OnSquadComponentDeath(int squadIndex) {
        var deadMember = membersAssigned.FirstOrDefault((member) => member.SquadIndex == squadIndex);
        if (deadMember != null)
            membersAssigned.Remove(deadMember);
    }

    protected virtual void StartBehaviour() {
        var members = manager.GetMembers(goalTemplates.Count);
        if(members.Count <= goalTemplates.Count) {
            members.ForEach((member) => member.SquadCompDeath += OnSquadComponentDeath);
            int startIndex = membersAssigned.Count;
            membersAssigned.AddRange(members);
            for(int i = 0; i < members.Count; i++) {
                membersAssigned[startIndex + i].AddGoalWith(goalTemplates[i]);
            }
        }
    }

    protected virtual void StopBehaviour() {
        foreach (var member in membersAssigned) {
            member.SquadCompDeath -= OnSquadComponentDeath;
            member.ResetGoal();
            manager.AddMember(member);
        }
        membersAssigned.Clear();
    }

}


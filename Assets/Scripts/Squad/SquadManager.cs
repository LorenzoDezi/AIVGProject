using GOAP;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public delegate void SquadChangeHandler(SquadComponent member);

public class SquadManager : MonoBehaviour {


    [SerializeField]
    private WorldStateKey squadObjectKey;
    private WorldState squadObjectWS;

    [SerializeField]
    private List<SquadComponent> squadMembers;
    public List<SquadComponent> CurrSquadMembers => squadMembers.ToList();

    [SerializeField]
    private List<SquadGoal> squadGoals;
    private int currGoalIndex;

    [SerializeField]
    private List<SquadBehaviour> squadBehaviourTemplates;
    private List<SquadBehaviour> squadBehaviours;
    private SquadSensor[] squadSensors;


    private WorldStates squadPerception;
    public WorldStates SquadPerception => squadPerception;

    public event SquadChangeHandler AddedMember;
    public event SquadChangeHandler RemovedMember;

    private void Awake() {
        squadPerception = new WorldStates();
        squadObjectWS = new WorldState(squadObjectKey, this.gameObject);
    }

    private void Start() {

        squadGoals.Sort(SquadGoal.SquadComparer);

        InitSquadMembers();
        InitBehaviours();
        InitSensors();
    }

    private void InitSensors() {
        squadSensors = GetComponents<SquadSensor>();
        for (int i = 0; i < squadSensors.Length; i++) {
            squadSensors[i].Init(this);
        }
    }

    private void InitBehaviours() {
        squadBehaviours = new List<SquadBehaviour>();
        foreach (var squadBehaviourTempl in squadBehaviourTemplates) {
            var squadBehaviour = Instantiate(squadBehaviourTempl);
            squadBehaviour.Init(this);
            squadBehaviours.Add(squadBehaviour);
        }
    }

    private void InitSquadMembers() {
        for (int i = 0; i < squadMembers.Count; i++) {

            var squadMember = squadMembers[i];
            squadMember.SquadIndex = i;
            squadMember.SquadCompDeath += OnSquadComponentDeath;

            if (squadMember[squadObjectKey] == null)
                squadMember.Add(squadObjectWS);
            else
                squadMember[squadObjectKey].GameObjectValue = this.gameObject;

            if (i < squadGoals.Count) {
                squadMember.AddGoalWith(squadGoals[i]);
                currGoalIndex = i;
            }
        }
    }

    private void OnSquadComponentDeath(int squadCompIndex) {
        RemovedMember.Invoke(RemoveMemberAt(squadCompIndex));
    }

    private SquadComponent RemoveMemberAt(int squadCompIndex) {

        if (squadCompIndex == currGoalIndex)
            currGoalIndex--;

        ShiftMembers(squadCompIndex);

        SquadComponent toBeRemoved = squadMembers[squadCompIndex];
        toBeRemoved.ResetGoal();
        toBeRemoved.SquadCompDeath -= OnSquadComponentDeath;
        squadMembers.RemoveAt(squadCompIndex);

        return toBeRemoved;
    }

    public void AddMember(SquadComponent squadMember) {
        squadMembers.Add(squadMember);
        squadMember.SquadIndex = squadMembers.Count - 1;
        squadMember.SquadCompDeath += OnSquadComponentDeath;
        AssignSquadGoal(squadMember);
        AddedMember.Invoke(squadMember);
    }

    private void ShiftMembers(int squadIndex) {
        for (int i = squadIndex + 1; i < squadMembers.Count; i++) {

            SquadComponent squadMember = squadMembers[i];
            squadMember.ResetGoal();

            squadMember.SquadIndex = i - 1;

            if (squadMember.SquadIndex < squadGoals.Count) {
                currGoalIndex = squadMember.SquadIndex;
                squadMember.AddGoalWith(squadGoals[squadMember.SquadIndex]);
            }
        }
    }


    private void AssignSquadGoal(SquadComponent squadMember) {
        if (currGoalIndex < squadGoals.Count - 1) {
            currGoalIndex++;
            squadMember.AddGoalWith(squadGoals[currGoalIndex]);
        }
    }

    public List<SquadComponent> GetMembers(int membersNeeded, WorldStates memberPreconditions) {

        List<SquadComponent> members = new List<SquadComponent>();

        for (int i = 0; i < squadMembers.Count && membersNeeded > 0; ) {
            var currMember = squadMembers[i];
            if(memberPreconditions.All((precondition) => currMember[precondition.Key].Equals(precondition))) {
                members.Add(RemoveMemberAt(i));
                membersNeeded--;
            } else {
                i++;
            }
        }

        return members;
    }

}

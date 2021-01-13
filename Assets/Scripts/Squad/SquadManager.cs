using GOAP;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public delegate void SquadChange(SquadComponent member);

public class SquadManager : MonoBehaviour {

    [SerializeField]
    private List<SquadComponent> squadMembers;
    public List<SquadComponent> CurrSquadMembers => squadMembers.ToList();

    [SerializeField]
    private List<SquadGoal> squadGoals;
    private int currentSquadGoalIndex;

    [SerializeField]
    private List<SquadBehaviour> squadBehaviours;
    private List<SquadSensor> squadSensors;


    private WorldStates squadPerception;
    public WorldStates SquadPerception => squadPerception;

    public event SquadChange AddedMember;
    public event SquadChange RemovedMember;

    private void Awake() {
        squadPerception = new WorldStates();
    }

    private void Start() {

        squadGoals.Sort(SquadGoal.SquadComparer);

        for(int i = 0; i < squadMembers.Count; i++) {

            var squadMember = squadMembers[i];
            squadMember.SquadGoalIndex = i;
            squadMember.SquadCompDeath += OnSquadComponentDeath;

            if (i < squadGoals.Count) {
                squadMember.AddGoalWith(squadGoals[i]);
                currentSquadGoalIndex = i;
            }
        }

        squadBehaviours.ForEach((squadBehaviour) => squadBehaviour.Init(this));

        squadSensors = GetComponents<SquadSensor>().ToList();
        squadSensors.ForEach((squadSensor) => squadSensor.Init(this));
    }

    private void OnSquadComponentDeath(int squadCompIndex) {
        RemovedMember.Invoke(RemoveMemberAt(squadCompIndex));
    }

    public SquadComponent RemoveMemberAt(int squadCompIndex) {

        //Resetting the other members to give the right priority goals
        for (int i = squadCompIndex + 1; i < squadMembers.Count; i++) {

            SquadComponent squadMember = squadMembers[i];
            squadMember.SquadGoalIndex = i - 1;

            squadMember.ResetGoal();
            if (squadMember.SquadGoalIndex < squadGoals.Count) {
                squadMember.AddGoalWith(squadGoals[squadMember.SquadGoalIndex]);
                currentSquadGoalIndex = squadMember.SquadGoalIndex;
            }
        }

        SquadComponent toBeRemoved = squadMembers[squadCompIndex];
        toBeRemoved.ResetGoal();
        squadMembers.RemoveAt(squadCompIndex);
        return toBeRemoved;
    }

    public void AddMember(SquadComponent squadMember) {
        squadMembers.Add(squadMember);
        squadMember.ResetGoal();
        AssignSquadGoal(squadMember);
        AddedMember.Invoke(squadMember);
    }

    private void AssignSquadGoal(SquadComponent squadMember) {
        if (currentSquadGoalIndex < squadGoals.Count - 1) {
            squadMember.SquadGoalIndex = currentSquadGoalIndex;
            squadMember.AddGoalWith(squadGoals[currentSquadGoalIndex]);
            currentSquadGoalIndex++;
        }
    }

    public List<SquadComponent> GetMembers(int count) {

        List<SquadComponent> members = new List<SquadComponent>();

        if (currentSquadGoalIndex < count - 1)
            return members;

        for(int i = 0; i < count; i++) {
            int curr_i = currentSquadGoalIndex - i;
            members.Add(RemoveMemberAt(curr_i));
        }

        currentSquadGoalIndex -= count;
        return members;
    }

}

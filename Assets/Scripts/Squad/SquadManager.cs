﻿using GOAP;
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

        for(int i = 0; i < squadMembers.Count; i++) {

            var squadMember = squadMembers[i];
            squadMember.SquadIndex = i;
            squadMember.SquadCompDeath += OnSquadComponentDeath;
            squadMember.UpdatePerception(squadObjectWS);

            if (i < squadGoals.Count) {
                squadMember.AddGoalWith(squadGoals[i]);
                currGoalIndex = i;
            }
        }

        squadBehaviours = new List<SquadBehaviour>();
        foreach(var squadBehaviourTempl in squadBehaviourTemplates) {
            var squadBehaviour = Instantiate(squadBehaviourTempl);
            squadBehaviour.Init(this);
            squadBehaviours.Add(squadBehaviour);
        }

        squadSensors = GetComponents<SquadSensor>();
        for(int i = 0; i < squadSensors.Length; i++) {
            squadSensors[i].Init(this);
        }
    }

    private void OnSquadComponentDeath(int squadCompIndex) {
        RemovedMember.Invoke(RemoveMemberAt(squadCompIndex));
    }

    public SquadComponent RemoveMemberAt(int squadCompIndex) {

        if (squadCompIndex == currGoalIndex)
            currGoalIndex--;

        ShiftMembers(squadCompIndex);

        SquadComponent toBeRemoved = squadMembers[squadCompIndex];
        toBeRemoved.ResetGoal();
        toBeRemoved.SquadCompDeath -= OnSquadComponentDeath;
        squadMembers.RemoveAt(squadCompIndex);

        return toBeRemoved;
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

    public void AddMember(SquadComponent squadMember) {
        squadMembers.Add(squadMember);
        squadMember.SquadIndex = squadMembers.Count - 1;
        squadMember.SquadCompDeath += OnSquadComponentDeath;
        AssignSquadGoal(squadMember);
        AddedMember.Invoke(squadMember);
    }

    private void AssignSquadGoal(SquadComponent squadMember) {
        if (currGoalIndex < squadGoals.Count - 1) {
            currGoalIndex++;
            squadMember.AddGoalWith(squadGoals[currGoalIndex]);
        }
    }

    public List<SquadComponent> GetMembers(int count) {

        List<SquadComponent> members = new List<SquadComponent>();
        for(int i = 0; i < count && squadMembers.Count > 0; i++) {
            members.Add(RemoveMemberAt(squadMembers.Count - 1));
        }

        return members;
    }

}

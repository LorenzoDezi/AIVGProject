using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadTactics : ScriptableObject
{
    [SerializeField]
    private List<SquadGoal> squadGoals;
    public List<SquadGoal> SquadGoals => squadGoals;
    //TODO: Vedere se è necessario / possibile / ha senso
}

using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadGoalComparer : IComparer<SquadGoal> {
    public int Compare(SquadGoal x, SquadGoal y) {
        return Convert.ToInt32(y.SquadPriority - x.SquadPriority);
    }
}

public class SquadGoal : Goal {

    [SerializeField]
    protected float squadPriority;
    public float SquadPriority => squadPriority;

    public static SquadGoalComparer SquadComparer { get; } = new SquadGoalComparer();
}

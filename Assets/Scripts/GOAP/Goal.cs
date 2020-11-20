using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GOAP {

    public class GoalComparer : IComparer<Goal> {
        public int Compare(Goal x, Goal y) {
            return Convert.ToInt32(x.Priority - y.Priority);
        }
    }


    public abstract class Goal : ScriptableObject {
        public static GoalComparer Comparer { get; } = new GoalComparer();

        [SerializeField]
        private WorldStates desiredStates;
        public WorldStates DesiredStates => desiredStates;

        [SerializeField]
        private int priority;
        public float Priority => priority;

        public abstract void UpdatePriority();
    }

}

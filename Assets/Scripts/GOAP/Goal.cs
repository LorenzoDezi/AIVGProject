using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace GOAP {

    public class GoalComparer : IComparer<Goal> {
        public int Compare(Goal x, Goal y) {
            return Convert.ToInt32(y.Priority - x.Priority);
        }
    }


    public abstract class Goal : ScriptableObject {

        public static GoalComparer Comparer { get; } = new GoalComparer();

        [SerializeField]
        private WorldStates desiredStates;
        public WorldStates DesiredStates => desiredStates;

        protected float priority;
        public float Priority => priority;

        [NonSerialized]
        public UnityEvent PriorityChanged = new UnityEvent(); 

        public virtual void Init(GameObject agentObj) {
            desiredStates = new WorldStates(desiredStates);
        }

        protected virtual void UpdatePriority() {
            PriorityChanged.Invoke();
        }
    }

}

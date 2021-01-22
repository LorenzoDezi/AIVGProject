using System;
using UnityEngine;

namespace GOAP {

    [Serializable]
    public class GoalPriorityUpdater {
        [SerializeField]
        private bool isWorldWS;
        [SerializeField]
        WorldStateKey refWSKey;
        WorldState referenceWS;

        public event StateChangedHandler PriorityChanged {
            add {
                referenceWS.StateChanged += value;
            }
            remove {
                referenceWS.StateChanged -= value;
            }
        }

        [SerializeField]
        private float priorityMultiplier;
        [SerializeField]
        private float maxPriority;
        [SerializeField]
        private bool inversePriority;

        public void Init(Agent agent) {
            var worldStates = isWorldWS ? World.WorldStates : agent.WorldPerception;
            referenceWS = worldStates[refWSKey];
            if (referenceWS == null) {
                referenceWS = new WorldState(refWSKey);
                worldStates.Add(referenceWS);
            }
        }

        public float GetPriority() {
            float priority = referenceWS.IntValue * priorityMultiplier;
            if (priority > maxPriority)
                priority = maxPriority;
            return inversePriority ? maxPriority - priority : priority;
        }
    }

}



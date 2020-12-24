using System;
using UnityEngine;

namespace GOAP {
    //TODO: Refactor all into GOal? Without GoalPriorityUpdater?
    [Serializable]
    public class GoalPriorityUpdater {
        [SerializeField]
        WorldState referenceWS;
        WorldState currReferenceWS;
        public WorldState CurrReferenceWS => currReferenceWS;

        [SerializeField]
        private float priorityMultiplier;
        [SerializeField]
        private bool inversePriority;

        public void Init(Agent agent) {
            var agentWorldStates = agent.WorldPerception;
            currReferenceWS = agentWorldStates[referenceWS.Key];
            if (currReferenceWS == null) {
                currReferenceWS = new WorldState(referenceWS);
                agentWorldStates.Add(currReferenceWS);
            }
        }

        public float GetPriority() {
            float priority = currReferenceWS.IntValue * priorityMultiplier;
            return inversePriority ? priorityMultiplier - priority : priority;
        }
    }

}



using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GOAP {
    [Serializable]
    public class WorldStates : IEnumerable<WorldState> {
        [SerializeField]
        private List<WorldState> states;
        private Dictionary<WorldStateKey, WorldState> stateDict;

        public int Count => states.Count;

        public WorldStates() {
            states = new List<WorldState>();
            stateDict = new Dictionary<WorldStateKey, WorldState>();
        }

        public WorldStates(WorldStates worldStates) : this() {
            foreach(WorldState state in worldStates) {
                if (!stateDict.ContainsKey(state.Key)) {
                    var copyState = new WorldState(state);
                    states.Add(copyState);
                    stateDict.Add(copyState.Key, copyState);
                }
            }
        }

        public static WorldStates operator + (WorldStates op1, WorldStates op2) {
            WorldStates result = new WorldStates(op1);
            op1.Update(op2);
            return result;
        }

        public WorldState this[WorldStateKey key] {
            get => stateDict.ContainsKey(key) ? stateDict[key] : null;
        }

        public void Update(WorldStates worldStates) {
            foreach (WorldState state in worldStates) {
                Update(state);
            }
        }

        public void Update(WorldState state) {
            if (stateDict.ContainsKey(state.Key))
                stateDict[state.Key].Update(state);
            else {
                states.Add(state);
                stateDict.Add(state.Key, state);
            }
        }

        public int SatisfactionCount(WorldStates toBeSatisfied) {
            int count = 0;
            foreach(WorldState state in toBeSatisfied) {
                if (states.Contains(state))
                    count += 1;
            }
            return count;
        }


        public bool Contains(WorldStates desiredStates) {
            return desiredStates.All(states.Contains);
        }

        public bool Contains(WorldState state) {
            return states.Contains(state);
        }

        public override bool Equals(object obj) {
            if(obj is WorldStates) {
                WorldStates otherWorldStates = (WorldStates) obj;
                return Contains(otherWorldStates) && otherWorldStates.Count == Count;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return 282409271 + EqualityComparer<List<WorldState>>.Default.GetHashCode(states);
        }

        IEnumerator<WorldState> IEnumerable<WorldState>.GetEnumerator() {
            return ((IEnumerable<WorldState>)states).GetEnumerator();
        }

        public IEnumerator GetEnumerator() {
            return ((IEnumerable<WorldState>)states).GetEnumerator();
        }
    }
}

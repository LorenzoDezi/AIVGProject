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
        public int Count => states.Count;

        public List<WorldStateValue> WorldStateValues =>
            states.Select((state) => state.Value).ToList();

        public WorldStates() {
            states = new List<WorldState>();
        }

        public WorldStates(WorldStates worldStates) : this() {
            foreach(WorldState state in worldStates)
                states.Add(new WorldState(state));
        }

        public void Update(WorldStates worldStates) {
            foreach (WorldState state in worldStates) {
                Update(state);
            }
        }

        public void Add(WorldState newState) {
            states.Add(newState);
        }

        public void Update(WorldState newState) {
            foreach(var state in states) {
                if(state.Key == newState.Key) {
                    state.Update(newState);
                    return;
                }
            }
            states.Add(newState);
        }

        public bool Contains(WorldStates desiredStates) {
            return desiredStates.All(Contains);
        }

        public bool Contains(WorldState state) {
            return state.Key != null && states.Contains(state);
        }

        public bool LinkedWith(WorldStates others) {
            return others.Count != 0 && others.Any(Contains);
        }

        public void Clear() {
            states.Clear();
        }

        public int SatisfactionCount(WorldStates toBeSatisfied) {
            int count = 0;
            foreach(WorldState state in toBeSatisfied) {
                if (Contains(state))
                    count += 1;
            }
            return count;
        }

        public WorldState this[WorldStateKey key] {
            get => states.FirstOrDefault((state) => state.Key == key);
        }

        public override bool Equals(object obj) {
            if(obj is WorldStates) {
                WorldStates otherWorldStates = (WorldStates) obj;
                return otherWorldStates.Count == Count && Contains(otherWorldStates);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return 282409271 + EqualityComparer<List<WorldState>>.Default.GetHashCode(states);
        }

        IEnumerator<WorldState> IEnumerable<WorldState>.GetEnumerator() {
            return states.GetEnumerator();
        }

        public IEnumerator GetEnumerator() {
            return states.GetEnumerator();
        }
    }
}

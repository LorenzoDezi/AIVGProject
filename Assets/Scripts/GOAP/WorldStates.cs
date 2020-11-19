using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GOAP {
    [Serializable]
    public class WorldStates {
        [SerializeField]
        private List<WorldState> states;

        public WorldStates() { }

        public WorldStates(WorldStates worldStates) {
            states = new List<WorldState>();
            foreach(WorldState state in worldStates.states) {
                states.Add(new WorldState(state));
            }
        }

        public bool Contains(WorldStates desiredStates) {
            return desiredStates.states.All(states.Contains);
        }

        public override bool Equals(object obj) {
            if(obj is WorldStates) {
                WorldStates worldStates = (WorldStates) obj;
                return Contains(worldStates) && worldStates.states.Count == states.Count;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return 282409271 + EqualityComparer<List<WorldState>>.Default.GetHashCode(states);
        }
    }
}

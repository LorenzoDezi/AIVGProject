using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GOAP {

    public enum WorldStateType {
        boolType, intType, gameObjectType
    }

    [Serializable]
    public class WorldState {
        [SerializeField]
        private WorldStateKey key = default;
        public WorldStateKey Key => key;
        [SerializeField]
        private WorldStateType type;
        public WorldStateType Type => type;

        [SerializeField]
        private int intValue;
        [SerializeField]
        private bool boolValue;
        [SerializeField]
        private GameObject gameObjectValue = default;

        private Dictionary<WorldStateType, Func<int>> valueDict;

        public WorldState() {
            valueDict = new Dictionary<WorldStateType, Func<int>>();
            valueDict.Add(WorldStateType.intType, () => intValue);
            valueDict.Add(WorldStateType.boolType, () => Convert.ToInt32(boolValue));
            valueDict.Add(WorldStateType.gameObjectType, () => gameObjectValue.GetInstanceID());
        }

        public WorldState(WorldStateKey key, int value) : this() {
            this.key = key;
            type = WorldStateType.intType;
            intValue = value;           
        }

        public WorldState(WorldStateKey key, bool value) : this() {
            this.key = key;
            type = WorldStateType.boolType;
            boolValue = value;
        }

        public WorldState(WorldState state) : this() {
            key = state.key;
            type = state.type;
            boolValue = state.boolValue;
            intValue = state.intValue;
            gameObjectValue = state.gameObjectValue;
        }

        public bool Match(WorldState other) {
            return other.Key == Key && 
                other.valueDict[other.Type] == valueDict[Type];
        }

        public override bool Equals(object obj) {
            if(!(obj is WorldState)) {
                return base.Equals(obj);
            } else {
                return Match((WorldState) obj);
            }
        }

        public override int GetHashCode() {
            var hashCode = -1095949941;
            hashCode = hashCode * -1521134295 + EqualityComparer<WorldStateKey>.Default.GetHashCode(key);
            hashCode = hashCode * -1521134295 + valueDict[type]();
            return hashCode;
        }
    }
}

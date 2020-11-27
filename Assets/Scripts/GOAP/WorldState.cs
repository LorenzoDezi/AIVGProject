using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GOAP {


    [Serializable]
    public class WorldState {
        [SerializeField]
        private WorldStateKey key = default;
        public WorldStateKey Key => key;

        [SerializeField]
        private int intValue;
        public int IntValue => intValue;
        [SerializeField]
        private bool boolValue;
        public bool BoolValue => boolValue;
        [SerializeField]
        private GameObject gameObjectValue = default;
        public GameObject GameObjectValue => gameObjectValue;

        private Dictionary<WorldStateType, Func<int>> valueDict;

        public WorldState() {
            valueDict = new Dictionary<WorldStateType, Func<int>>();
            valueDict.Add(WorldStateType.intType, () => intValue);
            valueDict.Add(WorldStateType.boolType, () => Convert.ToInt32(boolValue));
            valueDict.Add(WorldStateType.gameObjectType, () => gameObjectValue.GetInstanceID());
        }

        public WorldState(WorldStateKey key, int value) : this() {
            this.key = key;
            intValue = value;           
        }

        public WorldState(WorldStateKey key, bool value) : this() {
            this.key = key;
            boolValue = value;
        }

        public WorldState(WorldState state) : this() {
            Update(state);
        }

        public void Update(WorldState newWorldState) {
            key = newWorldState.key;
            intValue = newWorldState.intValue;
            boolValue = newWorldState.boolValue;
            gameObjectValue = newWorldState.gameObjectValue;
        }

        public bool Match(WorldState other) {
            return other.Key == Key && 
                other.valueDict[other.Key.Type]() == valueDict[Key.Type]();
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
            hashCode = hashCode * -1521134295 + valueDict[Key.Type]();
            return hashCode;
        }
    }
}

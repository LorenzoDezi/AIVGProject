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
        [SerializeField]
        private WorldStateType type;

        [SerializeField]
        private int intValue;
        [SerializeField]
        private bool boolValue;
        [SerializeField]
        private GameObject gameObjectValue = default;

        private Dictionary<WorldStateType, Func<int>> hashValues;

        public int Hash => key.Value ^ hashValues[type]();

        public WorldState() {
            hashValues = new Dictionary<WorldStateType, Func<int>>();
            hashValues.Add(WorldStateType.intType, () => intValue);
            hashValues.Add(WorldStateType.boolType, () => Convert.ToInt32(boolValue));
            hashValues.Add(WorldStateType.gameObjectType, () => gameObjectValue.GetInstanceID());
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

        public bool Match(WorldState other) {
            return other.Hash == this.Hash;
        }

        
    }
}

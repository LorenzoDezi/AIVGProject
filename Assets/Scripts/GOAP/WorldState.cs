using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace GOAP {

    [Serializable]
    public struct WorldStateValue {
        public WorldStateKey key;
        public int intValue;
        public float floatValue;
        public bool boolValue;
        public GameObject gameObjectValue;

        public override bool Equals(object obj) {
            if (!(obj is WorldStateValue)) {
                return base.Equals(obj);
            } else {
                WorldStateValue other = (WorldStateValue) obj;
                return other.key == key && 
                    WorldState.GetUniformValue(other) == WorldState.GetUniformValue(this);
            }
        }

        public override int GetHashCode() {
            var hashCode = -1095949941;
            hashCode = hashCode * -1521134295 + EqualityComparer<WorldStateKey>.Default.GetHashCode(key);
            hashCode = hashCode * -1521134295 + WorldState.GetUniformValue(this);
            return hashCode;
        }
    }

    [Serializable]
    public class WorldState {

        [SerializeField]
        private WorldStateValue value;

        public WorldStateKey Key => value.key;
        public WorldStateValue Value => value;

        public int IntValue {
            get => value.intValue;
            set {
                this.value.intValue = value;
            }
        }

        public float FloatValue {
            get => value.floatValue;
            set {
                this.value.floatValue = value;
                this.value.intValue = Convert.ToInt32(value);
            }
        }

        public bool BoolValue {
            get => value.boolValue;
            set {
                this.value.boolValue = value;
                this.value.intValue = Convert.ToInt32(value);
            }
        }

        public GameObject GameObjectValue {
            get => value.gameObjectValue;
            set {
                this.value.gameObjectValue = value;
            }
        }

        public delegate void StateChangedHandler();
        public StateChangedHandler StateChanged;

        #region constructors
        public WorldState(WorldStateKey key) {
            this.value.key = key;
        }

        public WorldState(WorldStateKey key, GameObject value) : this(key) {
            this.value.gameObjectValue = value;
        }

        public WorldState(WorldStateKey key, int value) : this(key) {
            this.value.intValue = value;
        }

        public WorldState(WorldStateKey key, float value) : this(key) {
            this.value.floatValue = value;
        }

        public WorldState(WorldStateKey key, bool value) : this(key) {
            this.value.boolValue = value;
        } 
        #endregion

        public WorldState(WorldState state) {
            Update(state);
        }

        public static int GetUniformValue(WorldStateValue value) {
            switch (value.key.Type) {
                case WorldStateType.boolType:
                    return Convert.ToInt32(value.boolValue);
                case WorldStateType.intType:
                    return value.intValue;
                case WorldStateType.floatType:
                    return Convert.ToInt32(value.floatValue);
                case WorldStateType.gameObjectType:
                    return value.gameObjectValue != null ?
                        value.gameObjectValue.GetInstanceID() : int.MaxValue;
                default:
                    return -1;
            }
        }

        public void Update(WorldState newWorldState) {
            value = newWorldState.value;
            StateChanged?.Invoke();
            //TODO: StateChanged invoked only on update... how to model this properly?
        }

        public bool Match(WorldState other) {
            return other.Key == Key && 
                GetUniformValue(value) == GetUniformValue(other.value);
        }

        public override bool Equals(object obj) {
            if(!(obj is WorldState)) {
                return base.Equals(obj);
            } else {
                return Match((WorldState) obj);
            }
        }

        public override int GetHashCode() {
            return value.GetHashCode();
        }
    }
}

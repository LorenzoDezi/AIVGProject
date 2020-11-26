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

    [CreateAssetMenu(fileName = "NewWorldStateKey", menuName = "GOAP/WorldStateKey")]
    public class WorldStateKey : ScriptableObject {

        [SerializeField]
        private WorldStateType type;
        public WorldStateType Type => type;

        public int Value { get; private set; }

        private void OnEnable() {
            Value = GetInstanceID();
        }

        public override bool Equals(object other) {
            if(!(other is WorldStateKey))
                return base.Equals(other);
            return ((WorldStateKey)other).Value == Value;
        }

        public override int GetHashCode() {
            var hashCode = -159790080;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            return hashCode;
        }
    }

}

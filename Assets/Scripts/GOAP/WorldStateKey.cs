using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GOAP {
    [CreateAssetMenu(fileName = "NewWorldStateKey", menuName = "GOAP/WorldStateKey")]
    public class WorldStateKey : ScriptableObject {
        public int Value { get; private set; }

        private void OnEnable() {
            Value = GetInstanceID();
        }

    }

}

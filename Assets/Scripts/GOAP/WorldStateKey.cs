using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GOAP {
    public class WorldStateKey : ScriptableObject {
        //TODO: Automatic counter of instances
        [SerializeField]
        private int value;

        public int Value => value;
    }

}

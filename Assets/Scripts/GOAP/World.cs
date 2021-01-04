using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GOAP {

    public class World : MonoBehaviour {

        private WorldStates worldStates;

        private static World instance;

        private void Awake() {
            if (instance != null) {
                Destroy(instance);
            }
            instance = this;
            worldStates = new WorldStates();
        }

        public static WorldStates WorldStates => instance.worldStates;

        public static void Update(WorldState state) {
            instance.worldStates.Update(state);
        }

    }

}




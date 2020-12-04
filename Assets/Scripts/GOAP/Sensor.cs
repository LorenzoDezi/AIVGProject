using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GOAP {

    [RequireComponent(typeof(Agent))]
    public abstract class Sensor : MonoBehaviour {
        //TODO: Add AgentSensor for single agent sensors, and not WORLD - if you decide to keep perception and World separated
        [SerializeField]
        protected WorldStateKey keyToUpdate;
        protected WorldState currWorldStateTracked;
        protected Agent agentToUpdate;

        protected virtual void Awake() {
            agentToUpdate = GetComponent<Agent>();
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOAP {

    public abstract class Action : ScriptableObject { 

        [SerializeField]
        protected WorldStates preconditions;
        public WorldStates Preconditions => preconditions;

        [SerializeField]
        protected WorldStates effects;
        public WorldStates Effects => effects;

        [SerializeField]
        protected float cost;

        protected Agent agent;

        public float Cost => cost;

        public delegate void EndActionHandler(bool isSuccessful);
        public event EndActionHandler EndAction;

        public virtual bool CheckProceduralConditions() {           
            return true;
        }

        public virtual void Init(GameObject agentGameObj) {
            this.preconditions = new WorldStates(Preconditions);
            this.effects = new WorldStates(Effects);
            this.agent = agentGameObj.GetComponent<Agent>();
        }

        public abstract bool Activate();

        public abstract void Deactivate();

        public abstract void Update();

        protected virtual void Terminate(bool success) {
            EndAction?.Invoke(success);
        }

    }

}



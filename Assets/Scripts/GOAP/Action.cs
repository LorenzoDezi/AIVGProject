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
        public float Cost => cost;

        public delegate void EndActionHandler(bool isSuccessful);
        public event EndActionHandler EndAction;

        public virtual bool CheckProceduralConditions() {           
            return true;
        }

        public virtual void Init(GameObject agentGameObj) {
        }

        public abstract bool Activate();

        public abstract void Deactivate();

        public abstract void Update();

        protected void Terminate(bool success) {
            EndAction?.Invoke(success);
        }

    }

}



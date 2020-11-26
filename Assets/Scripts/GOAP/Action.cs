using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOAP {

    public abstract class Action : ScriptableObject { 

        [SerializeField]
        private WorldStates preconditions;
        public WorldStates Preconditions => preconditions;

        [SerializeField]
        private WorldStates effects;
        public WorldStates Effects => effects;

        [SerializeField]
        private float cost;

        public float Cost => cost;
        public UnityEvent EndAction { get; }

        public Action() {
            EndAction = new UnityEvent();
        }

        public virtual bool CheckProceduralConditions() {           
            return true;
        }

        public abstract void Init(GameObject agentGameObj);

        public abstract void Activate();

        public abstract void Update();

        protected virtual void Terminate() {
            EndAction.Invoke();
        }

    }

}



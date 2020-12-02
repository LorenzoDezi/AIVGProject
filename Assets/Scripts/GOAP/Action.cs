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
        public UnityEvent EndAction { get; }

        public Action() {
            EndAction = new UnityEvent();
        }

        public virtual bool CheckProceduralConditions() {           
            return true;
        }

        public virtual void Init(GameObject agentGameObj) {
            this.preconditions = new WorldStates(Preconditions);
            this.effects = new WorldStates(Effects);
        }

        public abstract void Activate();

        public abstract void Deactivate();

        public abstract void Update();

        protected virtual void Terminate() {
            EndAction.Invoke();
        }

    }

}



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOAP {

    public class EndActionEvent : UnityEvent<bool> { }

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
        public EndActionEvent EndAction { get; }

        public Action() {
            EndAction = new EndActionEvent();
        }

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
            EndAction.Invoke(success);
        }

    }

}



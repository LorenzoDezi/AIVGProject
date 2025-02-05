﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GOAP {

    public class Agent : MonoBehaviour {

        #region Goals and actions attributes
        [SerializeField]
        private List<Action> actionTemplates = default;
        protected List<Action> actions;

        [SerializeField]
        private List<Goal> goalTemplates = default;
        protected List<Goal> goals;
        public List<Goal> Goals => goals;

        protected Queue<Action> actionQueue;
        protected Action currAction;
        public Action CurrAction => currAction; 
        #endregion

        [SerializeField]
        protected float replanInterval = 1f;
        [SerializeField]
        protected WorldStates worldPerception;
        public WorldStates WorldPerception => worldPerception;
        protected Planner planner;

        protected Coroutine checkPlanCoroutine;

#if UNITY_EDITOR
        public delegate void PlanCompletedHandler(List<Action> actions);
        public event PlanCompletedHandler PlanCompleted;
        public List<Action> ActionTemplates => actionTemplates;
#endif

        #region public methods
        public void Add(WorldState state) {
            worldPerception.Add(state);
        }

        public WorldState this[WorldStateKey key] {
            get => worldPerception[key];
        }

        public void Add(Goal goal) {
            goals.Add(goal);
            goal.Init(gameObject);
            goal.PriorityChanged += UpdateGoals;
            UpdateGoals();
        }

        public void Remove(Goal goal) {
            goals.Remove(goal);
            goal.PriorityChanged -= UpdateGoals;
            UpdateGoals();
        }

        public void Clear() {
            if (currAction != null) {
                currAction.Deactivate();
                currAction = null;
            }
            foreach (var goal in goals)
                Destroy(goal);
            foreach (var action in actions)
                Destroy(action);
            StopCoroutine(checkPlanCoroutine);
        } 
        #endregion

        #region monobehaviour calls
        protected virtual void Awake() {
            goals = new List<Goal>();
            actions = new List<Action>();
        }

        private void Start() {

            InitActions();
            planner = new Planner(actions);

            InitGoals();
            UpdateGoals();

            checkPlanCoroutine = StartCoroutine(CheckPlan());
        }

        protected virtual void Update() {
            currAction?.Update();
        }
        #endregion

        #region coroutines
        private IEnumerator CheckPlan() {

            var replanWaitFor = new WaitForSeconds(replanInterval);
            var nextFrameWait = new WaitForEndOfFrame();
            Action newAction;

            while (true) {

                newAction = null;

                foreach(Goal goal in goals.ToList()) {

                    if (worldPerception.Contains(goal.DesiredStates))
                        continue;

                    actionQueue = planner.Plan(goal.DesiredStates, worldPerception);
#if UNITY_EDITOR    
                    PlanCompleted?.Invoke(actionQueue.ToList());
#endif
                    if (actionQueue.Count > 0) {
                        newAction = actionQueue.Dequeue();
                        break;
                    }
                    yield return nextFrameWait;
                }

                if (newAction != currAction) {
                    DisableCurrAction();
                    currAction = newAction;
                    EnableCurrAction();
                }
                yield return replanWaitFor;
            }
        } 
        #endregion

        #region private methods
        private void InitActions() {
            for (int i = 0; i < actionTemplates.Count; i++) {
                actions.Add(Instantiate(actionTemplates[i]));
                actions[i].Init(gameObject);
            }
        } 

        private void EnableCurrAction() {

            if (currAction == null)
                return;

            if (currAction.Activate())
                currAction.EndAction += OnEndAction;
            else
                currAction = null;
        }

        private void DisableCurrAction() {

            if (currAction == null)
                return;

            currAction.Deactivate();
            currAction.EndAction -= OnEndAction;
            currAction = null;
        }

        private void OnEndAction(bool success) {

            DisableCurrAction();

            if(success && actionQueue.Count > 0) {
                currAction = actionQueue.Dequeue();
                EnableCurrAction();
            }
        }

        private void InitGoals() {
            for (int i = 0; i < goalTemplates.Count; i++) {

                var goal = Instantiate(goalTemplates[i]);
                goal.Init(gameObject);
                goal.PriorityChanged += UpdateGoals;

                goals.Add(goal);
            }
        }

        private void UpdateGoals() {
            goals.Sort(Goal.Comparer);
        }

        
        #endregion


    }
}



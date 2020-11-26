using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOAP {
    public class Agent : MonoBehaviour {
        [SerializeField]
        protected List<Action> actionTemplates;
        protected List<Action> actions;
        [SerializeField]
        protected List<Goal> goals;
        [SerializeField]
        protected float replanInterval = 1f;
        protected WorldStates worldPerception;

        protected Goal currGoal;
        protected Queue<Action> actionQueue;
        protected Action currAction;
        protected Planner planner;

        protected Coroutine checkPlanCoroutine;

        public void UpdatePerception(WorldState state) {
            worldPerception.Update(state);
            UpdateGoals();
        }

        #region monobehaviour calls
        private void Awake() {
            InitActions();
            planner = new Planner(actions);
        }

        protected virtual void Start() {
            foreach (Goal goal in goals) {
                goal.AgentWorldPerception = worldPerception;
            }
            UpdateGoals();
            checkPlanCoroutine = StartCoroutine(CheckPlan());
        }

        private void Update() {
            currAction?.Update();
        }
        #endregion

        #region coroutines
        private IEnumerator CheckPlan() {
            var replanWaitFor = new WaitForSeconds(replanInterval);
            while (true) {
                actionQueue = planner.Plan(currGoal, worldPerception);
                NextCurrAction();
                yield return replanWaitFor;
            }
        } 
        #endregion

        #region private methods
        private void InitActions() {
            actions = new List<Action>();
            for (int i = 0; i < actionTemplates.Count; i++) {
                actions.Add((Action)ScriptableObject.CreateInstance(actionTemplates[i].GetType()));
                actions[i].Init(gameObject);
            }
        }

        private void NextCurrAction() {
            if (actionQueue.Count == 0) {
                currAction = null;
                return;
            }
            currAction = actionQueue.Dequeue();
            currAction.EndAction.AddListener(OnEndAction);
        }

        private void OnEndAction() {
            currAction.EndAction.RemoveListener(OnEndAction);
            NextCurrAction();
        }

        private void UpdateGoals() {
            foreach (Goal goal in goals)
                goal.UpdatePriority();
            goals.Sort(Goal.Comparer);
            currGoal = goals.First();
        } 
        #endregion


    }
}



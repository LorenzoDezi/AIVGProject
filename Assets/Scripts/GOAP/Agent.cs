using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOAP {

    public class Agent : MonoBehaviour {

        #region Goals and actions
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

        public void UpdatePerception(WorldState state) {
            worldPerception.Update(state);
        }

        #region monobehaviour calls
        protected virtual void Awake() {
            InitActions();
            InitGoals();
            planner = new Planner(actions);
        }

        protected virtual void Start() {           
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
            while (true) {
                foreach(Goal goal in goals) {
                    actionQueue = planner.Plan(goal, worldPerception);
                    if(actionQueue.Count > 0) {
                        Action nextAction = actionQueue.Dequeue();
                        if(nextAction != currAction) {
                            DisableCurrAction();
                            currAction = nextAction;
                            EnableCurrAction();
                        }                       
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }
                yield return replanWaitFor;
            }
        } 
        #endregion

        #region private methods
        private void InitActions() {
            actions = new List<Action>();
            for (int i = 0; i < actionTemplates.Count; i++) {
                actions.Add(Instantiate(actionTemplates[i]));
                actions[i].Init(gameObject);
            }
        }

        private void InitGoals() {
            goals = new List<Goal>();
            for(int i = 0; i < goalTemplates.Count; i++) {
                goals.Add((Goal)ScriptableObject.CreateInstance(goalTemplates[i].GetType()));
                goals[i].Init(gameObject, goalTemplates[i]);
                goals[i].PriorityChanged.AddListener(UpdateGoals);
            }
        }

        private void EnableCurrAction() {
            currAction.Activate();
            currAction.EndAction.AddListener(OnEndAction);
        }

        private void DisableCurrAction() {
            if (currAction != null) {
                currAction.Deactivate();
                currAction.EndAction.RemoveListener(OnEndAction);
            }
        }

        private void OnEndAction() {
            DisableCurrAction();
            if(actionQueue.Count > 0) {
                currAction = actionQueue.Dequeue();
                EnableCurrAction();
            }
        }

        private void UpdateGoals() {
            goals.Sort(Goal.Comparer);
        }
        
        protected void Clear() {
            if(currAction != null) {
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


    }
}



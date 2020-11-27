using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOAP {

    public class Agent : MonoBehaviour {
        [SerializeField]
        private List<Action> actionTemplates = default;
        protected List<Action> actions;
        [SerializeField]
        private List<Goal> goalTemplates = default;
        protected List<Goal> goals;
        [SerializeField]
        protected float replanInterval = 1f;
        [SerializeField]
        protected WorldStates worldPerception;
        public WorldStates WorldPerception => worldPerception;

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
                actions.Add((Action) ScriptableObject.CreateInstance(actionTemplates[i].GetType()));
                actions[i].Init(gameObject, actionTemplates[i]);
            }
        }

        private void InitGoals() {
            goals = new List<Goal>();
            for(int i = 0; i < goalTemplates.Count; i++) {
                goals.Add((Goal)ScriptableObject.CreateInstance(goalTemplates[i].GetType()));
                goals[i].Init(gameObject);
            }
        }

        private void NextCurrAction() {
            if (actionQueue.Count == 0) {
                currAction = null;
                return;
            }
            Action nextAction = actionQueue.Dequeue();
            if(nextAction != currAction) {
                currAction = nextAction;
                currAction.Activate();
                currAction.EndAction.AddListener(OnEndAction);
            }            
        }

        private void OnEndAction() {
            currAction.Deactivate();
            currAction.EndAction.RemoveListener(OnEndAction);
            NextCurrAction();
        }

        private void UpdateGoals() {
            foreach (Goal goal in goalTemplates)
                goal.UpdatePriority();
            goalTemplates.Sort(Goal.Comparer);
            currGoal = goalTemplates.First();
        } 
        #endregion


    }
}



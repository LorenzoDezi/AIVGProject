using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOAP {
    public class Agent : MonoBehaviour {
        [SerializeField]
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

        private void Awake() {
            planner = new Planner(actions);
        }

        protected virtual void Start() {
            goals.Sort(Goal.Comparer);
            checkPlanCoroutine = StartCoroutine(CheckPlan());
        }

        IEnumerator CheckPlan() {
            var replanWaitFor = new WaitForSeconds(replanInterval);
            while(true) {
                actionQueue = planner.Plan(currGoal, worldPerception);
                yield return replanWaitFor;
            }
        }
    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAP {

    public class Planner {

        private PlanGraph graph = null;

        public Planner(List<Action> actions) {
            graph = new PlanGraph(actions);
        }

        public void AddAction(Action action) {
            graph.AddConnection(action);
        }

        public void RemoveAction(Action action) {
            graph.RemoveConnection(action);
        }

        public Queue<Action> Plan(Goal goal, WorldStates worldPerception) {
            WorldStates desiredStates = new WorldStates(goal.DesiredStates);
            WorldStates currentStates = new WorldStates(worldPerception);
            Action currAction;
            Stack<WorldStates> stateStack = new Stack<WorldStates>();
            stateStack.Push(currentStates);
            while(!currentStates.Contains(desiredStates)) {
                //TODO
            }
            return null;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAP {

    public class PlanNode {
        public WorldStates NodeStates { get; }
        public List<PlanConnection> PlanConnections { get; }
        public float CostSoFar { get; }

        public PlanNode(WorldStates states) {
            NodeStates = states;
        }

        public void AddConnection(PlanConnection connection) {
            PlanConnections.Add(connection);
        }

        public override bool Equals(object obj) {
            if(obj is PlanNode)
                return NodeStates.Equals(((PlanNode) obj).NodeStates);
            else if (obj is WorldStates)
                return NodeStates.Equals((WorldStates) obj);
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return -1976249345 + EqualityComparer<WorldStates>.Default.GetHashCode(NodeStates);
        }

        public void RemoveConnection(PlanConnection toRemove) {
            PlanConnections.Remove(toRemove);
        }
    }

    public class PlanConnection {
        public PlanNode FromNode;
        public PlanNode ToNode;
        public Action Action;
        public float Cost;

        public PlanConnection(PlanNode fromNode, PlanNode toNode, Action action) {
            FromNode = fromNode;
            ToNode = toNode;
            Action = action;
        }
    }

    public class PlanGraph {
        public List<PlanNode> Nodes { get; }
        List<PlanConnection> connections;

        public PlanGraph() {
            Nodes = new List<PlanNode>();
        }

        public PlanGraph(List<Action> actions) : this() {
            foreach (Action action in actions)
                AddConnection(action);
        }

        public void AddConnection(Action connectionAction) {
            PlanNode preconditionsNode = GetPlanNode(connectionAction.Preconditions);
            PlanConnection planConnection = new PlanConnection(
                preconditionsNode, GetPlanNode(connectionAction.Effects), connectionAction
            );
            connections.Add(planConnection);
            preconditionsNode.AddConnection(planConnection);
        }

        private PlanNode GetPlanNode(WorldStates worldStates) {
            foreach (PlanNode node in Nodes) {
                if (node.Equals(worldStates)) {
                    return node;
                }
            }
            PlanNode newNode = new PlanNode(worldStates);
            Nodes.Add(newNode);
            return newNode;
        }

        public void RemoveConnection(Action action) {
            PlanConnection connectionToRemove = null;
            foreach(PlanConnection connection in connections) {
                if (connection.Action == action) {
                    connectionToRemove = connection;
                    break;
                }
            }
            if(connectionToRemove != null) {
                connectionToRemove.FromNode.RemoveConnection(connectionToRemove);
                connections.Remove(connectionToRemove);
            }
        }      
    }

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

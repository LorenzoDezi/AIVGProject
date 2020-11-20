using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAP {

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
            foreach (PlanConnection connection in connections) {
                if (connection.Action == action) {
                    connectionToRemove = connection;
                    break;
                }
            }
            if (connectionToRemove != null) {
                connectionToRemove.FromNode.RemoveConnection(connectionToRemove);
                connections.Remove(connectionToRemove);
            }
        }
    }
}

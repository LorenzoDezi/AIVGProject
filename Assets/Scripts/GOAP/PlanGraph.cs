using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAP {

    public class PlanGraph {
        public List<PlanNode> Nodes { get; }

        public PlanGraph() {
            Nodes = new List<PlanNode>();
        }

        public PlanGraph(List<Action> actions) : this() {
            actions.ForEach((action) => Nodes.Add(new PlanNode(action)));
            Nodes.ForEach((node) => AddConnections(node));
        }

        public void AddConnections(PlanNode node) {
            List<PlanNode> connectedPlanNodes = GetConnectedNodesFrom(node);
            foreach(var connectedNode in connectedPlanNodes) {
                PlanConnection planConnection = new PlanConnection(node, connectedNode);
                node.AddConnection(planConnection);
            }
        }

        private List<PlanNode> GetConnectedNodesFrom(PlanNode planNode) {
            WorldStates preconditionsToSatisfy = planNode.Action.Preconditions;
            return Nodes.Where((node) => node != planNode && node.Action.Effects.LinkedWith(preconditionsToSatisfy)).ToList();
        }

        public void AddNode(Action action) {
            PlanNode newNode = new PlanNode(action);
            Nodes.Add(newNode);
            AddConnections(newNode);
        }

        public void RemoveNode(Action action) {
            PlanNode toBeRemoved = null;
            foreach(var node in Nodes) {
                if (node.Action == action) {
                    toBeRemoved = node;
                    break;
                }
            }
            Nodes.Remove(toBeRemoved);
        }

        public void ClearRecords() {
            Nodes.ForEach((node) => node.ClearRecord());
        }
    }
}

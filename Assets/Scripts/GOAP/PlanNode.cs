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
            if (obj is PlanNode)
                return NodeStates.Equals(((PlanNode)obj).NodeStates);
            else if (obj is WorldStates)
                return NodeStates.Equals((WorldStates)obj);
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return -1976249345 + EqualityComparer<WorldStates>.Default.GetHashCode(NodeStates);
        }

        public void RemoveConnection(PlanConnection toRemove) {
            PlanConnections.Remove(toRemove);
        }
    }
}

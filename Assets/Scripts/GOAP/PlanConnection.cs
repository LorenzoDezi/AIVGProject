using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAP {

    public class PlanConnection {
        public PlanNode FromNode;
        public PlanNode ToNode;
        public float Cost => ToNode.Action.Cost;

        public PlanConnection(PlanNode fromNode, PlanNode toNode) {
            FromNode = fromNode;
            ToNode = toNode;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAP {

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
}

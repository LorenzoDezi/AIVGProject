using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAP {

    public class PlanNode {

        public Action Action { get; }
        public WorldStates Preconditions => Action.Preconditions;
        public WorldStates Effects => Action.Effects;
        public List<PlanConnection> PlanConnections { get; }

        private PlanNodeRecord record;
        public PlanNodeRecord Record { get {
                if (record == null)
                    record = new PlanNodeRecord(this);
                return record;
            } }

        public PlanNode(Action nodeAction) {
            Action = nodeAction;
            PlanConnections = new List<PlanConnection>();
        }

        public void AddConnection(PlanConnection connection) {
            PlanConnections.Add(connection);
        }

        public bool Satisfy(WorldStates states) {
            return Action.Effects.Contains(states);
        }

        public void RemoveConnection(PlanConnection toRemove) {
            PlanConnections.Remove(toRemove);
        }

        public void ClearRecord() {
            Record.Reset();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAP {

    public class PlanNode {

        public Action Action { get; }

        private List<WorldStateValue> preconditions;
        public List<WorldStateValue> Preconditions => preconditions;

        private List<WorldStateValue> effects;
        public List<WorldStateValue> Effects => effects;

        public List<PlanConnection> PlanConnections { get; }

        private PlanNodeRecord record;
        public PlanNodeRecord Record {
            get {
                if (record == null)
                    record = new PlanNodeRecord(this);
                return record;
            }
        }

        public PlanNode(Action nodeAction) {
            Action = nodeAction;
            effects = Action.Effects.WorldStateValues;
            preconditions = Action.Preconditions.WorldStateValues;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GOAP {

    public class PlanNodeRecord : IComparable {

        private PlanNode node;
        public bool Closed { get; set; }
        public bool Open { get; set; }

        public WorldStates WorldPerception { get; set; }
        public WorldStates DesiredWorld { get; set; }

        public List<WorldStateValue> CurrStates { get; set; }
        public List<WorldStateValue> GoalStates { get; set; }

        public List<PlanConnection> Connections => node.PlanConnections;

        public PlanNodeRecord NextNode { get; set; }

        public Action Action => node.Action;

        public float CostSoFar { get; set; }
        public int HeuristicCost {
            get; set;
        }

        public float TotalCost => CostSoFar + HeuristicCost;
        public bool IsSatisfied => GoalStates.All(CurrStates.Contains);

        public PlanNodeRecord(PlanNode node) {
            this.node = node;
            CostSoFar = node.Action.Cost;
        }

        public void Reset() {
            HeuristicCost = 0;
            CostSoFar = node.Action.Cost;
            Closed = false;
            Open = false;
            NextNode = null;
            CurrStates = null;
            GoalStates = null;
        }

        public int CompareTo(object obj) {            
            return TotalCost.CompareTo(((PlanNodeRecord) obj).TotalCost);
        }
    }

    public class Planner {

        private PlanGraph graph = null;

        public Planner(List<Action> actions) {
            graph = new PlanGraph(actions);
        }

        public void AddAction(Action action) {
            graph.AddNode(action);
        }

        public void RemoveAction(Action action) {
            graph.RemoveNode(action);
        }

        public int HeuristicEstimate(PlanNodeRecord record) {

            int satisfiedCount = 0;
            foreach(var desiredState in record.GoalStates) {
                if (record.CurrStates.Contains(desiredState))
                    satisfiedCount++;
            }

            return record.GoalStates.Count - satisfiedCount;
        }

        public Queue<Action> Plan(WorldStates desiredWorld, WorldStates worldPerception) {

            List<PlanNodeRecord> open = GetOpenList(desiredWorld, worldPerception);
            bool found = false;
            PlanNodeRecord curr = null;

            while (open.Count > 0) {

                curr = open.Min();
                if (curr.IsSatisfied) {
                    found = true;
                    break;
                }

                foreach (var conn in curr.Connections)
                    ProcessConnection(curr, conn, open);

                open.Remove(curr);
                curr.Closed = true;
            }

            Queue<Action> actions = ExtractActions(found, curr);
            graph.ClearRecords();
            return actions;
        }

        private void ProcessConnection(PlanNodeRecord curr, PlanConnection conn, 
            List<PlanNodeRecord> open) {

            PlanNode connectedNode = conn.ToNode;
            if (!connectedNode.Action.CheckProceduralConditions())
                return;

            float newCostSoFar = curr.CostSoFar + conn.Cost;
            PlanNodeRecord nodeRecord = connectedNode.Record;
            bool convenient = nodeRecord.CostSoFar > newCostSoFar;

            if (nodeRecord.Closed) {

                if (convenient) {
                    nodeRecord.Closed = false;                 
                    open.Add(nodeRecord);
                } else
                    return;

            } else if (nodeRecord.Open) {

                if (!convenient)
                    return;

            } else {

                nodeRecord.Open = true;
                open.Add(nodeRecord);
            }

            nodeRecord.CurrStates = curr.CurrStates.GetUpdatedWith(connectedNode.Effects);
            nodeRecord.GoalStates = curr.GoalStates.GetUpdatedWith(connectedNode.Preconditions);
            nodeRecord.HeuristicCost = HeuristicEstimate(nodeRecord);
            nodeRecord.NextNode = curr;
            nodeRecord.CostSoFar = newCostSoFar;
        }

        private List<PlanNodeRecord> GetOpenList(WorldStates desiredWorld, WorldStates worldPerception) {

            List<PlanNodeRecord> open = new List<PlanNodeRecord>();
            List<WorldStateValue> worldPerceptionVal = worldPerception.WorldStateValues;
            List<WorldStateValue> desiredWorldVal = desiredWorld.WorldStateValues;
            foreach (PlanNode node in graph.Nodes) {
                PlanNodeRecord nodeRecord = node.Record;
                nodeRecord.WorldPerception = worldPerception;
                nodeRecord.DesiredWorld = desiredWorld;
                if (node.Satisfy(desiredWorld)
                    && node.Action.CheckProceduralConditions()) {
                    nodeRecord.CurrStates = worldPerceptionVal.GetUpdatedWith(node.Effects);
                    nodeRecord.GoalStates = desiredWorldVal.GetUpdatedWith(node.Preconditions);
                    nodeRecord.HeuristicCost = HeuristicEstimate(nodeRecord);
                    nodeRecord.Open = true;
                    open.Add(nodeRecord);
                }
            }

            return open;
        }

        private static Queue<Action> ExtractActions(bool planFound, PlanNodeRecord startingNode) {
            Queue<Action> actions = new Queue<Action>();
            if (planFound) {
                actions.Enqueue(startingNode.Action);
                while (startingNode.NextNode != null) {
                    startingNode = startingNode.NextNode;
                    actions.Enqueue(startingNode.Action);
                }
            }
            return actions;
        }
    }
}

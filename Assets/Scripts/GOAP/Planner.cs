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

        public List<WorldStateValue> CurrGoalStates { get; set; }
        public List<WorldStateValue> DesiredGoalStates { get; set; }

        public List<PlanConnection> Connections => node.PlanConnections;

        public PlanNodeRecord NextNode { get; set; }

        public Action Action => node.Action;

        public float CostSoFar { get; set; }
        public int HeuristicCost {
            get; set;
        }

        public float TotalCost => CostSoFar + HeuristicCost;
        public bool IsSatisfied => DesiredGoalStates.All(CurrGoalStates.Contains);

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
            CurrGoalStates = null;
            DesiredGoalStates = null;
        }

        public int CompareTo(object obj) {            
            return TotalCost.CompareTo(((PlanNodeRecord) obj).TotalCost);
        }
    }

    public class Planner {

        private PlanGraph graph = null;
        private List<WorldStateValue> heuristicCache;

        public Planner(List<Action> actions) {
            graph = new PlanGraph(actions);
            heuristicCache = new List<WorldStateValue>();
        }

        public void AddAction(Action action) {
            graph.AddNode(action);
        }

        public void RemoveAction(Action action) {
            graph.RemoveNode(action);
        }

        public int HeuristicEstimate(PlanNodeRecord record) {

            int satisfiedCount = 0;
            foreach(var desiredState in record.DesiredGoalStates) {
                if (record.CurrGoalStates.Contains(desiredState))
                    satisfiedCount++;
            }

            return record.DesiredGoalStates.Count - satisfiedCount;
        }

        public Queue<Action> Plan(Goal goal, WorldStates worldPerception) {

            List<PlanNodeRecord> open = GetOpenList(goal, worldPerception);
            bool found = false;
            PlanNodeRecord curr = null;

            while (open.Count > 0) {

                curr = open.Min();
                if (curr.IsSatisfied) {
                    found = true;
                    break;
                }

                foreach (var conn in curr.Connections) {
                    var connectedNodeRecord = GetNextNode(curr, conn);
                    if (connectedNodeRecord != null)
                        open.Add(connectedNodeRecord);
                }

                open.Remove(curr);
                curr.Closed = true;
            }

            Queue<Action> actions = ExtractActions(found, curr);
            graph.ClearRecords();
            return actions;
        }

        private PlanNodeRecord GetNextNode(PlanNodeRecord curr, PlanConnection conn) {

            PlanNode connectedNode = conn.ToNode;
            if (!connectedNode.Action.CheckProceduralConditions())
                return null;

            float newCostSoFar = curr.CostSoFar + conn.Cost;
            PlanNodeRecord nodeRecord = connectedNode.Record;
            bool convenient = nodeRecord.CostSoFar > newCostSoFar;

            if (nodeRecord.Closed) {
                if (convenient)
                    nodeRecord.Closed = false;
                else
                    return null;
            } else if (nodeRecord.Open) {
                if (!convenient)
                    return null;
            } else
                nodeRecord.Open = true;

            nodeRecord.CurrGoalStates = curr.CurrGoalStates.GetUpdatedWith(connectedNode.Effects);
            nodeRecord.DesiredGoalStates = curr.DesiredGoalStates.GetUpdatedWith(connectedNode.Preconditions);
            nodeRecord.HeuristicCost = HeuristicEstimate(nodeRecord);
            nodeRecord.NextNode = curr;
            nodeRecord.CostSoFar = newCostSoFar;
            return nodeRecord;
        }

        private List<PlanNodeRecord> GetOpenList(Goal goal, WorldStates worldPerception) {

            List<PlanNodeRecord> open = new List<PlanNodeRecord>();
            var world = World.WorldStates.WorldStateValues;
            var desiredWorld = goal.DesiredStates.WorldStateValues;
            world.UpdateWith(worldPerception.WorldStateValues);

            foreach (PlanNode node in graph.Nodes) {
                if (node.Satisfy(goal.DesiredStates)
                    && node.Action.CheckProceduralConditions()) {
                    PlanNodeRecord nodeRecord = node.Record;
                    nodeRecord.CurrGoalStates = world.GetUpdatedWith(node.Effects);
                    nodeRecord.DesiredGoalStates = desiredWorld.GetUpdatedWith(node.Preconditions);
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

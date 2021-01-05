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

        public Dictionary<WorldStateKey, WorldStateValue> CurrGoalStates { get; set; }
        public Dictionary<WorldStateKey, WorldStateValue> DesiredGoalStates { get; set; }

        public List<PlanConnection> Connections => node.PlanConnections;

        public PlanNodeRecord NextNode { get; set; }

        public Action Action => node.Action;

        public float CostSoFar { get; set; }
        public int HeuristicCost {
            get; set;
        }

        public float TotalCost => CostSoFar + HeuristicCost;
        public bool IsSatisfied => DesiredGoalStates.All(
            (keyValuePair) => CurrGoalStates.ContainsKey(keyValuePair.Key) 
                && CurrGoalStates[keyValuePair.Key].Equals(keyValuePair.Value)
        );

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

        public Planner(List<Action> actions) {
            graph = new PlanGraph(actions);
        }

        public void AddAction(Action action) {
            graph.AddNode(action);
        }

        public void RemoveAction(Action action) {
            graph.RemoveNode(action);
        }

        //TODO: Refactor and heuristic cache in some way
        //public int HeuristicEstimate(WorldStates agentPerception, WorldStates desiredStates, 
        //    PlanNode node) {            
        //    WorldStates updatedPerception = agentPerception.Updated(node.Effects);
        //    return updatedPerception.Count - updatedPerception.SatisfactionCount(
        //        desiredStates.Updated(node.Preconditions));
        //}

        public Queue<Action> Plan(Goal goal, WorldStates worldPerception) {
            List<PlanNodeRecord> open = new List<PlanNodeRecord>();
            var world = World.WorldStates.WorldStateValues;
            world.UpdateWith(worldPerception.WorldStateValues);
            foreach (PlanNode node in graph.Nodes) {
                if (node.Satisfy(goal.DesiredStates) 
                    && node.Action.CheckProceduralConditions()) {
                    PlanNodeRecord nodeRecord = node.Record;
                    nodeRecord.CurrGoalStates = new Dictionary<WorldStateKey, WorldStateValue>(world).UpdateWith(node.Effects);
                    nodeRecord.DesiredGoalStates = new Dictionary<WorldStateKey, WorldStateValue>(goal.DesiredStates.WorldStateValues).UpdateWith(node.Preconditions);
                    //nodeRecord.HeuristicCost = HeuristicEstimate(world, goal.DesiredStates, node);
                    nodeRecord.Open = true;
                    open.Add(nodeRecord);
                }
            }
            bool found = false;
            PlanNodeRecord curr = null;
            while (open.Count > 0) {

                curr = open.Min();                
                if (curr.IsSatisfied) {
                    found = true;
                    break;
                }

                foreach (var conn in curr.Connections) {
                    PlanNode connectedNode = conn.ToNode;
                    if (!connectedNode.Action.CheckProceduralConditions())
                        continue;
                    int heuristicCost = 0; //HeuristicEstimate(curr.CurrGoalStates, curr.DesiredGoalStates, connectedNode);
                    float newCostSoFar = curr.CostSoFar + conn.Cost;
                    float newTotalCost = newCostSoFar + heuristicCost;
                    PlanNodeRecord nodeRecord = connectedNode.Record;
                    //Closed check
                    if (nodeRecord.Closed) {
                        if (nodeRecord.TotalCost <= newTotalCost)
                            continue;
                        nodeRecord.Closed = false;
                        //Open check
                    } else if (nodeRecord.Open) {
                        if (nodeRecord.TotalCost <= newTotalCost)
                            continue;
                        //Unvisited
                    } else {
                        nodeRecord.CurrGoalStates = new Dictionary<WorldStateKey, WorldStateValue>(curr.CurrGoalStates).UpdateWith(connectedNode.Effects);
                        nodeRecord.DesiredGoalStates = new Dictionary<WorldStateKey, WorldStateValue>(curr.DesiredGoalStates).UpdateWith(connectedNode.Preconditions);
                        nodeRecord.HeuristicCost = heuristicCost;
                        nodeRecord.NextNode = curr;
                        nodeRecord.CostSoFar = newCostSoFar;
                        nodeRecord.Open = true;
                        open.Add(nodeRecord);
                    }
                }
                open.Remove(curr);
                curr.Closed = true;
            }
            Queue<Action> actions = ExtractActions(found, curr);
            graph.ClearRecords();
            return actions;
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

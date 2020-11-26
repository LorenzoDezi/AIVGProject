using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAP {

    public class PlanNodeRecord : IComparable {

        private PlanNode node;
        public bool Closed { get; set; }
        public bool Open { get; set; }

        private WorldStates worldPerception;
        public WorldStates WorldPerception {
            get => worldPerception;
            set {
                worldPerception = value + node.Effects;
            }
        }

        private WorldStates desiredStates;
        public WorldStates DesiredStates {
            get => desiredStates;
            set {
                desiredStates = value + node.Preconditions;
            }
        }

        public List<PlanConnection> Connections => node.PlanConnections;

        public PlanNodeRecord NextNode { get; set; }

        public Action Action => node.Action;

        public float CostSoFar { get; set; }
        public int HeuristicCost {
            get; set;
        }

        public float TotalCost => CostSoFar + HeuristicCost;
        public bool IsSatisfied => worldPerception.Contains(DesiredStates);

        public PlanNodeRecord(PlanNode node) {
            this.node = node;
        }

        public void Reset() {
            HeuristicCost = 0;
            CostSoFar = 0f;
            Closed = false;
            Open = false;
            NextNode = null;
            worldPerception = null;
            desiredStates = null;
        }

        public int CompareTo(object obj) {
            return TotalCost.CompareTo(obj);
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
        public int HeuristicEstimate(WorldStates worldPerception, WorldStates desiredStates, 
            PlanNode node) {
            return (worldPerception + node.Effects).SatisfactionCount(desiredStates + node.Preconditions);
        }

        public Queue<Action> Plan(Goal goal, WorldStates worldPerception) {
            List<PlanNodeRecord> open = new List<PlanNodeRecord>();
            foreach (PlanNode node in graph.Nodes) {
                if (node.Satisfy(goal.DesiredStates)) {
                    PlanNodeRecord nodeRecord = node.Record;
                    nodeRecord.WorldPerception = worldPerception;
                    nodeRecord.DesiredStates = goal.DesiredStates;
                    nodeRecord.HeuristicCost = HeuristicEstimate(worldPerception, goal.DesiredStates, node);
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
                    int heuristicCost = HeuristicEstimate(curr.WorldPerception, curr.DesiredStates, connectedNode);
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
                        nodeRecord.WorldPerception = curr.WorldPerception;
                        nodeRecord.DesiredStates = curr.DesiredStates;
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace GOAP.Editor {

    public class AgentEditorContainer {

        private List<ActionEditorNode> nodes;
        private float horizontalSpacing = 25f;
        private float verticalSpacing = 100f;

        private PlanGraph graph;

        public AgentEditorContainer(Agent agent) {

            nodes = new List<ActionEditorNode>();

            foreach (var action in agent.ActionTemplates)
                action.Init(agent.gameObject);
            graph = new PlanGraph(agent.ActionTemplates);
            int nRow = 5;
            Vector3 startPosition = (Vector3.up + Vector3.right) * horizontalSpacing;
            Vector3 currPosition = startPosition;
            for (int i = 0; i < graph.Nodes.Count; i++) {
                var actionNode = graph.Nodes[i];
                var node = new ActionEditorNode(actionNode);
                node.Position = currPosition;
                if (i > 0 && i % nRow == 0)
                    currPosition = startPosition + Vector3.up * (i / nRow) * (node.Height + verticalSpacing);
                else
                    currPosition += Vector3.right * (horizontalSpacing + node.Width);
                nodes.Add(node);                
            }
            UpdateConnections();

        }

        public void UpdateConnections() {
            
            foreach(var node in nodes) {
                node.Connections.Clear();
                foreach(var connection in node.ActionNode.PlanConnections) {
                    var actionEditorNode = nodes.First(
                        (n) => n.ID == connection.ToNode.Action.GetInstanceID()
                    );
                    node.Connections.Add(actionEditorNode);
                }
            }
            
        }

        public ActionEditorNode GetAt(Vector2 mousePosition) {
            foreach (var node in nodes) {
                if (node.IsAt(mousePosition)) {
                    return node;
                }
            }
            return null;
        }

        public void Draw(AgentEditorWindow window, AgentEditorSelectedEffects effectsChosen) {

            if (effectsChosen.Effects.Count == 0)
                return;

            foreach(var node in nodes) {
                if(node.ActionNode.Satisfy(effectsChosen.Effects)) {
                    node.Draw(Color.cyan);
                    node.DrawPlans();
                }
            }
        }

    }

}

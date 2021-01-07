using System;
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
            //TODO: Positions 
            for (int i = 0; i < graph.Nodes.Count; i++) {
                var actionNode = graph.Nodes[i];
                var node = new ActionEditorNode(actionNode);
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

        public void Draw(AgentEditorWindow window) {

            foreach(var node in nodes) {
                node.Draw();
                node.DrawLines();
            }
        }

    }

}

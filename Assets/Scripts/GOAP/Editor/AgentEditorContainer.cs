using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

namespace GOAP.Editor {

    public class AgentEditorContainer {

        private List<ActionEditorNode> nodes;
        private float distanceBetweenNodes = 50f;
        private PlanGraph graph;

        public AgentEditorContainer(Agent agent) {
            nodes = new List<ActionEditorNode>();
            graph = new PlanGraph(agent.ActionTemplates);

            Vector3 position = (Vector3.right + Vector3.up) * distanceBetweenNodes;
            foreach (var actionNode in graph.Nodes) {
                var node = new ActionEditorNode(actionNode);
                node.Position = position;
                nodes.Add(node);
                position += Vector3.right * (distanceBetweenNodes + node.Width);
            }
        }

        public void Draw(AgentEditorWindow window) {
            //TODO
            for(int i = 0; i < nodes.Count; i++) {
                nodes[i].Draw(i);
            }
        }

    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GOAP.Editor {

    public class AgentEditorWindow : EditorWindow {

        private int lastAgentID;
        private Agent selectedAgent;
        private AgentEditorContainer agentContainer;

        enum UserAction { AddNode, DeleteNode }

        [MenuItem("Window/AgentEditor")]
        static void ShowEditor() {
            var window = (AgentEditorWindow) GetWindow(typeof(AgentEditorWindow));
            window.minSize = new Vector2(800, 600);
        }

        private void OnGUI() {

            selectedAgent = Selection.activeTransform?.GetComponent<Agent>();
            if(selectedAgent == null) {
                DrawMessage("Select a gameObject with an Agent component!");
                return;
            } else if (selectedAgent.GetInstanceID() != lastAgentID) {
                lastAgentID = selectedAgent.GetInstanceID();
                agentContainer = new AgentEditorContainer(selectedAgent);
            }

            //TODO: Try placing a button to refresh if modifies occurs (see Refresh())
            DrawEditor();
        }

        private void Refresh() {

        }

        private void DrawMessage(string message) {
            BeginWindows();
            var centerRect = new Rect(position.width / 4f, position.height / 4f, position.width / 2f, position.height / 2f);
            GUILayout.BeginArea(centerRect);

            var style = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter };
            GUILayout.Box(message, style, GUILayout.Width(position.width / 2f), GUILayout.Height(50f));

            GUILayout.EndArea();
            EndWindows();
        }

        private void DrawEditor() {

            

            BeginWindows();

            agentContainer.Draw(this);

            EndWindows();
        }

        //private void DrawLines() {
        //    if (container != null) {
        //        for (int i = 0; i < container.nodes.Count; i++) {
        //            if (container.nodes[i].nextNode != null) {
        //                ConnectLine(container.nodes[i].nodeRect, container.nodes[i].nextNode.nodeRect);
        //            }
        //        }
        //    }
        //}

        //private void ConnectLine(Rect start, Rect end) {
        //    Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height + 0.5f);
        //    Vector3 endPos = new Vector3(end.x + end.width * 0.5f, end.y + end.height * 0.5f);

        //    //TODO: tan depending on real direction between start and end position
        //    Vector3 startTan = startPos + (Vector3.right * 50f);
        //    Vector3 endTan = endPos + (Vector3.left * 50f);

        //    Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 2f);
        //}
    }

}



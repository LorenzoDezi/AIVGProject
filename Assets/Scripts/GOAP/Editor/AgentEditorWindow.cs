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

        private Vector2 scrollPosition;
        private Vector2 mousePosition;

        enum UserAction { AddNode, DeleteNode }

        [MenuItem("Window/AgentEditor")]
        static void ShowEditor() {
            var window = (AgentEditorWindow)GetWindow(typeof(AgentEditorWindow));
            window.minSize = new Vector2(800, 600);
        }

        private void OnGUI() {

            selectedAgent = Selection.activeTransform?.GetComponent<Agent>();
            if (selectedAgent == null) {
                DrawMessage("Select a gameObject with an Agent component!");
                return;
            } else if (selectedAgent.GetInstanceID() != lastAgentID) {
                lastAgentID = selectedAgent.GetInstanceID();
                agentContainer = new AgentEditorContainer(selectedAgent);
            }

            ProcessInput();
            agentContainer.UpdateConnections();
            //TODO: Try placing a button to refresh if modifies occurs (see Refresh())
            DrawEditor();
        }

        private void ProcessInput() {

            Event e = Event.current;
            mousePosition = e.mousePosition;
            if (agentContainer != null && e.type == EventType.MouseDown && e.button == 1) {
                OpenContextMenuUsing(e);
            }

        }

        private void OpenContextMenuUsing(Event e) {
            GenericMenu menu = new GenericMenu();
            GUIContent menuItemContent;
            GenericMenu.MenuFunction menuItemFunction;
            if(agentContainer.GetAt(mousePosition + scrollPosition) != null) {
                menuItemContent = new GUIContent("Highlight plans");
                menuItemFunction = HighlightPlans;
            } else {
                menuItemContent = new GUIContent("Stop showing plans");
                menuItemFunction = StopShowingPlans;
            }
            menu.AddItem(menuItemContent, false, menuItemFunction);
            menu.ShowAsContext();
            e.Use();
        }

        private void HighlightPlans() {           
            agentContainer.SelectNodeAt(mousePosition + scrollPosition);
            agentContainer.ShowSelectedNodePlans(true);
        }

        private void StopShowingPlans() {
            agentContainer.ShowSelectedNodePlans(false);
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
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            //This label is needed to make the scrollArea work, at least i think
            GUILayout.Label(GUIContent.none, GUILayout.Height(this.position.height * 3f),
                GUILayout.Width(this.position.width * 3f));
            BeginWindows();
            agentContainer.Draw(this);
            EndWindows();
            EditorGUILayout.EndScrollView();
        }


    }

}



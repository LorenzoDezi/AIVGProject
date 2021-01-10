using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GOAP.Editor {

    public class AgentEditorSelectedEffects : ScriptableObject {
        [SerializeField]
        private WorldStates effects;
        public WorldStates Effects => effects;

    }

    public class AgentEditorWindow : EditorWindow {

        private int lastAgentID;
        private Agent selectedAgent;
        private AgentEditorContainer agentContainer;
        private Vector2 scrollPosition;
        private Vector2 mousePosition;

        private Vector2 headerSize;
        private GUIStyle headerStyle;

        private AgentEditorSelectedEffects selectedEffects;
        private SerializedObject selectedEffectsData;


        [MenuItem("Window/AgentEditor")]
        static void ShowEditor() {
            var window = (AgentEditorWindow)GetWindow(typeof(AgentEditorWindow));
            window.selectedEffects = new AgentEditorSelectedEffects();
            window.selectedEffectsData = new SerializedObject(window.selectedEffects);
            window.headerStyle = new GUIStyle();
            
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
            mousePosition = e.mousePosition + scrollPosition - new Vector2(headerSize.x, 0f);
            if (agentContainer != null && e.type == EventType.MouseDown && e.button == 1) {
                OpenContextMenuUsing(e);
            }

        }

        private void OpenContextMenuUsing(Event e) {
            GenericMenu menu = new GenericMenu();
            GUIContent menuItemContent;
            GenericMenu.MenuFunction menuItemFunction;
            if(agentContainer.GetAt(mousePosition) != null) {
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
            agentContainer.SelectNodeAt(mousePosition);
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
            
            DrawHeader();
            DrawAgent();
        }

        private void DrawHeader() {
            headerSize.x = Mathf.Clamp(position.width / 4f, 100f, 275f);
            headerSize.y = position.height;
            headerStyle.normal.background = MakeTex(
                Convert.ToInt32(headerSize.x), Convert.ToInt32(headerSize.y), new Color(0.6f, 0.6f, 0.6f, 1f));
            GUILayout.BeginArea(new Rect(Vector2.zero, headerSize), headerStyle);
            selectedEffectsData.Update();
            EditorGUILayout.PropertyField(selectedEffectsData.FindProperty("effects"), GUILayout.Width(headerSize.x - 20f));
            selectedEffectsData.ApplyModifiedProperties();
            GUILayout.EndArea();
        }

        private void DrawAgent() {
            GUILayout.BeginArea(new Rect(new Vector2(headerSize.x, 0f), new Vector2(position.width - headerSize.x, position.height)));
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            //This label is needed to make the scrollArea work, at least i think
            GUILayout.Label(GUIContent.none, GUILayout.Width(position.width * 2f));
            BeginWindows();
            agentContainer.Draw(this);
            EndWindows();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private Texture2D MakeTex(int width, int height, Color col) {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }



    }

}



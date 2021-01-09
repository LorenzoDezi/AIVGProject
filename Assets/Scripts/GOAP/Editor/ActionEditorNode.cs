using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GOAP.Editor {

    public class ActionEditorNode {

        private Color bgColor;
        private Color selectedBgColor;
        private Color currentBgColor;
        private Rect nodeRect;
        private GUIStyle buttonStyle;
        private Vector2 scrollPosition = Vector2.zero;
        private bool expanded;

        public int ID { get; private set; }
        public PlanNode ActionNode { get; private set; }
        public List<ActionEditorNode> Connections { get; private set; }

        public Vector3 Position {
            get => nodeRect.position;
            set => nodeRect.position = value;
        }

        public float Width => nodeRect.width;
        public float Height => nodeRect.height;

        public float Cost => ActionNode.Action.Cost;
        private bool isSelected;
        public bool IsSelected {
            get {
                return isSelected;
            }
            set {
                isSelected = value;
                currentBgColor = isSelected ? selectedBgColor : bgColor;
            }
        }


        public ActionEditorNode(PlanNode actionNode) {
            this.ActionNode = actionNode;
            this.ID = actionNode.Action.GetInstanceID();
            Connections = new List<ActionEditorNode>();
            SetRect();

            SetupBgColors();
            buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter };
        }

        private void SetupBgColors() {
            bgColor = GUI.backgroundColor;
            float bgColorAlpha = GUI.backgroundColor.a;
            bgColorAlpha -= 0.75f;
            bgColor.a = bgColorAlpha;
            selectedBgColor = GUI.backgroundColor;
            currentBgColor = bgColor;
        }

        private void SetRect() {
            if(expanded) {
                nodeRect.width = 300f;
                nodeRect.height = 200f;
            } else {
                nodeRect.width = 300f;
                nodeRect.height = 100f;
            }
        }

        private void OnDraw(int id) {
            Color original = GUI.backgroundColor;
            GUI.backgroundColor = currentBgColor;
            if(GUILayout.Button(expanded ? "Hide Inspector" : "Show Inspector", buttonStyle, GUILayout.Width(100f), GUILayout.Height(25f))) {
                expanded = !expanded;
                SetRect();
            }
            if(expanded) {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                var actionEditor = UnityEditor.Editor.CreateEditor(ActionNode.Action);
                actionEditor.DrawDefaultInspector();

                EditorGUILayout.EndScrollView();
            }
            GUI.DragWindow();
            GUI.backgroundColor = original;

        }

        public bool IsAt(Vector2 mousePosition) {
            return nodeRect.Contains(mousePosition);
        }

        public void Draw() {
            Color original = GUI.backgroundColor;
            GUI.backgroundColor = currentBgColor;
            nodeRect = GUILayout.Window(ID, nodeRect, OnDraw, ActionNode.Action.name);
            GUI.backgroundColor = original;
        }


        public void Select() {
            IsSelected = true;
            foreach(var conn in Connections) {
                conn.Select();
            }
        }

        public void DrawPlans() {
            foreach(var conn in Connections) {                
                ConnectLine(nodeRect, conn.nodeRect, conn.Cost, Color.red);
                conn.DrawPlans();
            }
        }

        private void ConnectLine(Rect start, Rect end, float cost, Color color) {

            Vector2 direction = (end.position - start.position).normalized;

            Handles.DrawBezier(
                start.center + new Vector2(direction.x * start.width/2f, direction.y * start.height/2f), 
                end.center - new Vector2(direction.x * end.width/2f, direction.y * end.height/2f),
                start.center + direction,
                end.center + direction,
                color,
                null,
                2f
            );
            
            GUILayout.BeginArea(new Rect( start.center + (end.center - start.center)/2f, new Vector2(50f, 50f)));
            EditorGUILayout.LabelField("" + cost);
            GUILayout.EndArea();

        }

    }

}



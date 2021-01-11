using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GOAP.Editor {

    public class ActionEditorNode {

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

        public Texture2D ArrowTex { get; set; }

        public float Width => nodeRect.width;
        public float Height => nodeRect.height;

        public float Cost => ActionNode.Action.Cost;


        public ActionEditorNode(PlanNode actionNode) {
            this.ActionNode = actionNode;
            this.ID = actionNode.Action.GetInstanceID();
            Connections = new List<ActionEditorNode>();
            SetRect();
            buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter };
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

        }

        public bool IsAt(Vector2 mousePosition) {
            return nodeRect.Contains(mousePosition);
        }

        public void Draw() {
            nodeRect = GUILayout.Window(ID, nodeRect, OnDraw, ActionNode.Action.name);
        }

        public void Draw(Color color) {
            Color original = GUI.backgroundColor;
            GUI.backgroundColor = color;
            Draw();
            GUI.backgroundColor = original;
        }

        public void DrawPlans() {
            foreach(var conn in Connections) {                
                conn.Draw();
                DrawArrow(nodeRect, conn.nodeRect, conn.Cost, Color.red);
                conn.DrawPlans();
            }
        }

        private void DrawArrow(Rect start, Rect end, float cost, Color color) {

            Vector2 direction = (end.position - start.position).normalized;
            Vector2 arrowStart = start.center + new Vector2(direction.x * start.width / 2f, direction.y * start.height / 2f);
            Vector2 arrowEnd = end.center - new Vector2(direction.x * end.width / 2f, direction.y * end.height / 2f);

            Handles.DrawAAPolyLine(
                10f,
                arrowStart,
                arrowEnd
            );

            direction = arrowEnd - arrowStart;
            float arrowSize = 10f;
            arrowStart += direction / 2f;
            direction.Normalize();
            Handles.DrawAAConvexPolygon(
                    arrowStart,
                    arrowStart + (direction + direction.RotatedBy(90f)) * arrowSize,
                    arrowStart + (direction + direction.RotatedBy(-90f)) * arrowSize
            );

            GUILayout.BeginArea(new Rect( start.center + (end.center - start.center)/2f, new Vector2(50f, 50f)));
            EditorGUILayout.LabelField("" + cost);
            GUILayout.EndArea();

        }

    }

}



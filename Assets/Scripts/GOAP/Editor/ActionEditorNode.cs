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
        public bool Visited {
            get => ActionNode.Record.Open || ActionNode.Record.Closed;
        }
        public PlanNode ActionNode { get; private set; }
        public List<ActionEditorNode> Connections { get; private set; }

        public Vector3 Position {
            get => nodeRect.position;
            set => nodeRect.position = value;
        }

        public float Width => nodeRect.width;
        public float Height => nodeRect.height;

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
            //TODO: write all sequence possibles and the total cost

            GUI.DragWindow();
        }

        public void Draw() {           
            nodeRect = GUI.Window(ID, nodeRect, OnDraw, ActionNode.Action.name);           
        }

        public void DrawLines() {
            foreach(var conn in Connections) {
                ConnectLine(nodeRect, conn.nodeRect);
            }
        }

        private void ConnectLine(Rect start, Rect end) {
            Handles.DrawLine(start.position, end.position);
        }
    }

}



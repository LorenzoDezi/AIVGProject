using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GOAP.Editor {

    public class ActionEditorNode {
        private Rect nodeRect;
        private GUIStyle style;
        private PlanNode actionNode;
        private Vector2 scrollPosition = Vector2.zero;

        public Vector3 Position {
            get => nodeRect.position;
            set => nodeRect.position = value;
        }

        public float Width => nodeRect.width;
        public float Height => nodeRect.height;

        public ActionEditorNode nextNode;
        public ActionEditorNode prevNode;

        public ActionEditorNode(PlanNode actionNode) {
            this.actionNode = actionNode;
            nodeRect.width = 300f;
            nodeRect.height = 200f;
            style = new GUIStyle(GUI.skin.box);
        }

        private void OnDraw(int id) {

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            var actionEditor = UnityEditor.Editor.CreateEditor(actionNode.Action);
            actionEditor.DrawDefaultInspector();

            EditorGUILayout.EndScrollView();

            GUI.DragWindow();
        }

        public void Draw(int id) {           
            nodeRect = GUI.Window(id, nodeRect, OnDraw, actionNode.Action.name);           
        }
    }

}



using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GOAP.Editor {
    [CustomPropertyDrawer(typeof(WorldState))]
    public class WorldStateDrawer : PropertyDrawer {

        private Vector2 verticalSpacing = Vector2.up * 20f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property == null)
                return;
            EditorGUI.BeginProperty(position, label, property);
            position.height = position.height / 5;
            Rect valuePosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.indentLevel++;
            RenderLabels(position);
            EditorGUI.indentLevel--;           
            valuePosition.position += verticalSpacing;
            EditorGUI.PropertyField(valuePosition, property.FindPropertyRelative("value.key"), GUIContent.none);
            valuePosition.position += verticalSpacing;
            RenderPropertyValue(valuePosition, property);
            EditorGUI.EndProperty();
        }

        private void RenderLabels(Rect position) {
            position.position += verticalSpacing;
            EditorGUI.LabelField(position, "Key");
            position.position += verticalSpacing;
            EditorGUI.LabelField(position, "Value");
        }

        private void RenderPropertyValue(Rect position, SerializedProperty property) {
            WorldStateKey key = (WorldStateKey) property.FindPropertyRelative("value.key").objectReferenceValue;
            if(key != null) {
                WorldStateType type = key.Type;
                switch (type) {
                    case WorldStateType.boolType:
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("value.boolValue"), GUIContent.none);
                        break;
                    case WorldStateType.intType:
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("value.intValue"), GUIContent.none);
                        break;
                    case WorldStateType.floatType:
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("value.floatValue"), GUIContent.none);
                        break;
                    case WorldStateType.gameObjectType:
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("value.gameObjectValue"), GUIContent.none);
                        break;
                }
            }           
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return base.GetPropertyHeight(property, label) * 5f;
        }

    }
}
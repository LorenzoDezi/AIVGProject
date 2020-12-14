using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOAP;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyDebug))]
[CanEditMultipleObjects]
public class EnemyDebugEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        serializedObject.Update();
        EnemyDebug enemyDebug = (EnemyDebug) target;
        if (enemyDebug.agent == null || enemyDebug.isDead || enemyDebug.agent.Goals == null)
            return;
        GUILayout.Label("Goal list and priorities");
        foreach(var goal in enemyDebug.agent.Goals) {
            GUILayout.Label(goal.name + ": " + goal.Priority);
        }
        CarriageAndReturn(1);
        GUILayout.Label("Current Plan");
        foreach(var action in enemyDebug.currentPlan) {
            GUILayout.Label("   " + action.name);
        }
        CarriageAndReturn(1);
        GUILayout.Label("Current Action: " + enemyDebug.agent.CurrAction?.name);
        CarriageAndReturn(1);
        GUILayout.Label("WorldPerception: ");
        foreach(WorldState worldState in enemyDebug.agent.WorldPerception) {
            GUILayout.Label("Key: " + worldState.Key.Name);
            DrawValue(worldState);
        }
    }

    private void CarriageAndReturn(int times) {
        for(int i = 0; i < times; i++)
            GUILayout.Label("");
    }

    private void DrawValue(WorldState state) {
        switch (state.Key.Type) {
            case WorldStateType.boolType:
                GUILayout.Label("    Value: " + state.BoolValue);
                break;
            case WorldStateType.intType:
                GUILayout.Label("    Value: " + state.IntValue);
                break;
            case WorldStateType.floatType:
                GUILayout.Label("    Value: " + state.FloatValue);
                break;
            case WorldStateType.gameObjectType:
                GUILayout.Label("    Value: " + state.GameObjectValue.name);
                break;
        }
    }
}
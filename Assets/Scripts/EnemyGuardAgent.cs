using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuardAgent : Agent
{
    [SerializeField]
    private List<Transform> patrolPoints;

    protected override void Awake() {
        base.Awake();
        foreach(var action in actions) {
            if (action is PatrolAction)
                ((PatrolAction) action).PatrolPoints = patrolPoints;
        }        
    }

    public void OnDeath() {
        Clear();
    }
}

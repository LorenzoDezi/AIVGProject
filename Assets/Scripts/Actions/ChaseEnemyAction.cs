using UnityEngine;

[CreateAssetMenu(fileName = "ChaseEnemyAction", menuName = "GOAP/Actions/ChaseEnemyAction")]
public class ChaseEnemyAction : ShootAction {

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);
    }

    public override bool Activate() {

        if (!base.Activate())
            return false;

        navComponent.Target = target;
        return true;
    }

    public override void Deactivate() {
        navComponent.Target = null;
    }
    
}


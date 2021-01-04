using UnityEngine;

[CreateAssetMenu(fileName = "ChaseEnemyAction", menuName = "GOAP/Actions/ChaseEnemyAction")]
public class ChaseEnemyAction : ShootAction {

    private NavigationComponent navComponent;

    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);

        navComponent = agentGameObj.GetComponent<NavigationComponent>();
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


using UnityEngine;

[CreateAssetMenu(fileName = "KnifeAttackAction", menuName = "GOAP/Actions/KnifeAttackAction")]
public class KnifeAttackAction : GOAP.Action {

    private NavigationComponent navComponent;
    private EnemyVisualSensor visualSensor;
    private KnifeController knifeController;
    private CharacterController charController;

    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);

        knifeController = agentGameObj.GetComponentInChildren<KnifeController>();
        navComponent = agentGameObj.GetComponent<NavigationComponent>();
        visualSensor = agentGameObj.GetComponent<EnemyVisualSensor>();
        charController = agentGameObj.GetComponent<CharacterController>();
    }

    public override bool Activate() {

        if (!visualSensor.IsEnemySpotted)
            return false;
        navComponent.Target = visualSensor.VisibleEnemy;
        visualSensor.CurrWeaponRange = knifeController.WeaponRange;
        knifeController.IsAttacking = true;
        return true;
    }

    public override void Deactivate() {
        navComponent.Target = null;
        knifeController.IsAttacking = false;
    }

    public override void Update() {
        charController.AimAt(navComponent.Target.position);
    }
}


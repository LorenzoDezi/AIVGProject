using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOAP;
using UnityEngine;

[CreateAssetMenu(fileName = "CloseDistanceAction", menuName = "GOAP/Actions/CloseDistanceAction")]
public class CloseDistanceAction : GOAP.Action {

    private EnemyVisualSensor enemyVisualSensor;
    private CharacterController characterController;
    private NavigationComponent navComponent;

    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);

        enemyVisualSensor = agentGameObj.GetComponent<EnemyVisualSensor>();
        characterController = agentGameObj.GetComponent<CharacterController>();
        navComponent = agentGameObj.GetComponent<NavigationComponent>();

    }

    public override bool Activate() {

        enemyVisualSensor.EnemyNearEvent.AddListener(OnEnemyNear);
        navComponent.Target = enemyVisualSensor.VisibleEnemy.transform;
        return true;
    }

    private void OnEnemyNear() {
        Terminate(true);
    }

    public override void Deactivate() {
        enemyVisualSensor.EnemyNearEvent.RemoveListener(OnEnemyNear);
    }

    public override void Update() {
        characterController.AimAt(navComponent.Target.position);
    }


}


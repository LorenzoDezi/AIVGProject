using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ChaseEnemyAction", menuName = "GOAP/Actions/ChaseEnemyAction")]
public class ChaseEnemyAction : GOAP.Action {

    private EnemyVisualSensor enemyVisualSensor;
    private NavigationComponent navComponent;

    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);

        enemyVisualSensor = agentGameObj.GetComponent<EnemyVisualSensor>();
        navComponent = agentGameObj.GetComponent<NavigationComponent>();
    }

    public override bool Activate() {

        if (!enemyVisualSensor.EnemySpotted)
            return false;

        navComponent.Target = enemyVisualSensor.VisibleEnemy.transform;
        return true;
    }

    public override void Deactivate() {
        navComponent.Target = null;
    }

    public override void Update() {
        
    }
}


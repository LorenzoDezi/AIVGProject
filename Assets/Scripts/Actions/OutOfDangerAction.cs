﻿using UnityEngine;

[CreateAssetMenu(fileName = "OutOfDangerAction", menuName = "GOAP/Actions/OutOfDangerAction")]
public class OutOfDangerAction : GOAP.Action {

    private NavigationComponent navComp;
    private CharacterController charController;
    private DangerSensor dangerSensor;
    private Transform transform;
    [SerializeField]
    private int maxSearchOutOfDangerIterations = 5;

    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);

        navComp = agentGameObj.GetComponent<NavigationComponent>();
        dangerSensor = agentGameObj.GetComponent<DangerSensor>();
        charController = agentGameObj.GetComponent<CharacterController>();
        transform = agentGameObj.transform;
    }

    public override bool Activate() {
        Vector3? destination = GetDestination();
        if(destination.HasValue) {
            charController.AimAt(destination.Value);
            navComp.MoveTo(destination.Value);
            navComp.PathCompleted.AddListener(OnPathCompleted);
            return true;
        }
        return false;
    }

    private Vector3? GetDestination() {
        Vector3? destination = null;
        Vector2 dangerDirection = (transform.position - dangerSensor.DangerSource).normalized;
        Vector3 rotatedDangerDirection = Vector3.zero;
        for (int i = 0; i < maxSearchOutOfDangerIterations; i++) {

            dangerDirection.RotatedBy(Random.Range(0f, 90f), ref rotatedDangerDirection);
            destination = dangerSensor.DangerSource +
                rotatedDangerDirection * (dangerSensor.DangerRadius + 4f);

            var node = AstarPath.active.GetNearest(destination.Value).node;
            if (node.Walkable)
                break;
            else
                destination = null;
        }
        return destination;
    }

    private void OnPathCompleted(bool isCompleted) {
        if(isCompleted) {
            agent.UpdatePerception(Effects);
            Terminate(true);
        } else {
            Vector3? destination = GetDestination();
            if (destination.HasValue)
                navComp.MoveTo(destination.Value);
            else
                Terminate(false);
        }
    }

    public override void Deactivate() {
        navComp.PathCompleted.RemoveListener(OnPathCompleted);
    }

    public override void Update() {
        
    }
}


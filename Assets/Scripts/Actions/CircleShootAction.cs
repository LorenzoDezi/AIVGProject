using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "CircleShootAction", menuName = "GOAP/Actions/CircleShootAction")]
public class CircleShootAction : ShootAction {

    [SerializeField]
    private float distance = 5f;
    [SerializeField]
    private int maxSearchFirePointIteration = 5;

    private float switchFirePointInterval = 3f;
    private float timeSinceLastSwitchFirePoint;

    protected Vector3 movementTargetPos;

    protected virtual void SetMovementTargetPosition() {
        movementTargetPos = target.position;
    }

    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);

        visualSensor.CurrWeaponRange = Mathf.Infinity;
    }

    public override bool Activate() {

        if (!base.Activate())
            return false;
        timeSinceLastSwitchFirePoint = switchFirePointInterval;
        return true;
    }

    public override void Update() {

        base.Update();

        if(timeSinceLastSwitchFirePoint >= switchFirePointInterval) {
            SetMovementTargetPosition();
            Vector3? possibleFirePoint = GetFirePoint();
            if (possibleFirePoint.HasValue) {
                navComponent.MoveTo(possibleFirePoint.Value);
                timeSinceLastSwitchFirePoint = 0f;
            } else {
                Terminate(false);
                return;
            }
        }

        timeSinceLastSwitchFirePoint += Time.deltaTime;
    }

    

    private Vector3? GetFirePoint() {

        int iteration = 0;
        while (iteration < maxSearchFirePointIteration) {
            Vector3 point = movementTargetPos;
            Vector2 displacement = UnityEngine.Random.insideUnitCircle * distance;
            point.x += displacement.x;
            point.y += displacement.y;
            if (!Physics2D.Linecast(point, target.position, obstacleLayerMask)) {
                var node = AstarPath.active.GetNearest(point).node;
                if (node.Walkable)
                    return (Vector3) node.position;
            }
            iteration++;
        }
        return null;        
    }

}


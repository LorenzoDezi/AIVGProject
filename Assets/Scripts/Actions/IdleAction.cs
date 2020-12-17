using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleAction", menuName = "GOAP/Actions/IdleAction")]
public class IdleAction : GOAP.Action {

    private CharacterController charController;
    [SerializeField]
    private float timeToSwitchLook = 5f;
    private float timeSinceLastSwitchLook = 0f;
    [SerializeField]
    private List<Vector2> lookDirections;
    private int currLookDirection;

    public override void Init(GameObject agentGameObj) {

        base.Init(agentGameObj);
        charController = agentGameObj.GetComponent<CharacterController>();

        var transform = agentGameObj.transform;
        if (lookDirections.Count == 0) {
            lookDirections.Add(transform.position + transform.right);
            return;
        }
        for (int i = 0; i < lookDirections.Count; i++)
            lookDirections[i] = (Vector2) transform.position + lookDirections[i];
    }

    public override bool Activate() {

        currLookDirection = 0;
        charController.AimAt(lookDirections[currLookDirection]);
        timeSinceLastSwitchLook = 0f;

        return true;
    }

    public override void Deactivate() {
        
    }

    public override void Update() {

        if(timeSinceLastSwitchLook >= timeToSwitchLook) {

            currLookDirection++;
            if (currLookDirection == lookDirections.Count)
                currLookDirection = 0;
            charController.AimAt(lookDirections[currLookDirection]);

            timeSinceLastSwitchLook = 0f;

        } else {
            timeSinceLastSwitchLook += Time.deltaTime;
        }
    }
}


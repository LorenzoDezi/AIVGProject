using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOAP;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolAction", menuName = "GOAP/Actions/PatrolAction")]
public class PatrolAction : GOAP.Action {

    public List<Transform> PatrolPoints { get; set; }
    private int currPatrolIndex;
    private NavigationComponent navigationComp;
    private Transform transform;

    public override void Init(GameObject agentGameObj, WorldStates preconditions, WorldStates effects, float cost) {
        this.preconditions = new WorldStates(preconditions);
        this.effects = new WorldStates(effects);
        this.cost = cost;
        navigationComp = agentGameObj.GetComponent<NavigationComponent>();
        navigationComp.PathCompleted.AddListener(OnPathCompleted);
        transform = agentGameObj.transform;
    }

    public override void Activate() {
        Transform closerPatrolPoint = null;
        float minSqrDistance = Mathf.Infinity;
        for(int i = 0; i < PatrolPoints.Count; i++) {
            float currSqrDistance = Vector3.SqrMagnitude(PatrolPoints[i].position - transform.position);
            if ( currSqrDistance < minSqrDistance) {
                minSqrDistance = currSqrDistance;
                closerPatrolPoint = PatrolPoints[i];
                currPatrolIndex = i;
            }
        }
        navigationComp.MoveTo(closerPatrolPoint);
    }

    public override void Deactivate() {
        navigationComp.Stop();
        navigationComp.PathCompleted.RemoveListener(OnPathCompleted);
    }

    public override void Update() {
        
    }

    public void OnPathCompleted() {
        currPatrolIndex++;
        if (currPatrolIndex >= PatrolPoints.Count)
            currPatrolIndex = 0;
        navigationComp.MoveTo(PatrolPoints[currPatrolIndex]);
    }

    
}


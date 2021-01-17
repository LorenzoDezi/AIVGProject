using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "CoordSearchAction", menuName = "GOAP/Actions/CoordSearchAction")]
public class CoordSearchAction : SearchAction {

    [SerializeField]
    private WorldStateKey squadObjectKey;

    private WorldStates perception;

    private SearchCoordinator searchCoordinator;

    private bool currSearchPointReached;

    public override void Init(GameObject agentGameObj) {
        base.Init(agentGameObj);
        perception = agentGameObj.GetComponent<Agent>().WorldPerception;
    }

    public override bool Activate() {

        if (!base.Activate())
            return false;

        var squadObject = perception[squadObjectKey]?.GameObjectValue;
        if (squadObject == null)
            return false;

        searchCoordinator = squadObject.GetComponent<SearchCoordinator>();
        Vector3? possibleSearchPoint = searchCoordinator.GetSearchPoint();
        if (possibleSearchPoint.HasValue) {
            SetSearchPoint(possibleSearchPoint.Value);
        }
        return true;
    }

    public override void Deactivate() {

        base.Deactivate();

        if(!currSearchPointReached)
            searchCoordinator.Add(searchPoint);
    }

    protected override void OnPathCompleted(bool success) {
        if(success) {

            currSearchPointReached = true;

            Vector3? possibleSearchPoint = searchCoordinator.GetSearchPoint();
            if (possibleSearchPoint.HasValue) {
                SetSearchPoint(possibleSearchPoint.Value);
                currSearchPointReached = false;
            } else {
                Terminate(true);
            }
        }
    }
}


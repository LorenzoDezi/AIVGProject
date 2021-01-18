using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SquadNeedCoverSensor : SquadSensor {

    [SerializeField]
    private WorldStateKey needCoverKey;

    private List<SquadComponent> membersWhoNeedCover;
    public List<SquadComponent> MembersWhoNeedCover => membersWhoNeedCover;

    private void Awake() {
        membersWhoNeedCover = new List<SquadComponent>();
    }

    public override void Init(SquadManager manager) {
        base.Init(manager);
        foreach(var member in squadMembers) {
            WorldState needCoverWS = member[needCoverKey];
            if(needCoverWS == null) {
                needCoverWS = new WorldState(needCoverKey, false);
                member.UpdatePerception(needCoverWS);               
            }
            needCoverWS.StateChanged += GetOnStateChange(member);
        }
    }

    public SquadComponent GetMemberWhoNeedCover(Vector3 protectorPosition) {
        int chosenMemberIndex = -1;
        float minSqrDistance = Mathf.Infinity;

        for(int i = 0; i < membersWhoNeedCover.Count; i++) {
            SquadComponent currMember = membersWhoNeedCover[i];
            float currSqrDistance = Vector3.SqrMagnitude(protectorPosition - currMember.Transform.position);
            if(currSqrDistance < minSqrDistance) {
                minSqrDistance = currSqrDistance;
                chosenMemberIndex = i;
            }
        }

        if (chosenMemberIndex > -1) {
            SquadComponent member = membersWhoNeedCover[chosenMemberIndex];
            membersWhoNeedCover.RemoveAt(chosenMemberIndex);
            return member;
        }
        return null;
    }

    public void AddMemberWhoNeedCover(SquadComponent squadComp) {
        WorldState squadNeedCover = manager.SquadPerception[needCoverKey];
        if(squadNeedCover != null && squadNeedCover.BoolValue) {
            membersWhoNeedCover.Add(squadComp);
        }
    }

    private StateChangedHandler GetOnStateChange(SquadComponent memberToCover) {
        return () => {
            WorldState needCover = memberToCover[needCoverKey];
            WorldState squadNeedCover = manager.SquadPerception[needCoverKey];
            if (needCover.BoolValue) {
                membersWhoNeedCover.Add(memberToCover);
                manager.SquadPerception.Update(needCover);
            } else {                
                membersWhoNeedCover.Remove(memberToCover);
                if (membersWhoNeedCover.Count == 0)
                    manager.SquadPerception.Update(needCover);
            }
        };
    }

}


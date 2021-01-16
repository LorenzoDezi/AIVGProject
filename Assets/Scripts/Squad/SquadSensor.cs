using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class SquadSensor : MonoBehaviour {

    protected SquadManager manager;
    protected List<SquadComponent> squadMembers;


    public virtual void Init(SquadManager manager) {
        this.manager = manager;
        this.squadMembers = manager.CurrSquadMembers;
        manager.AddedMember += OnAddMember;
        manager.RemovedMember += OnRemoveMember;
    }

    protected virtual void OnAddMember(SquadComponent newMember) {
        if (!squadMembers.Contains(newMember)) {
            squadMembers.Add(newMember);
        }
    }

    protected virtual void OnRemoveMember(SquadComponent removedMember) {
        squadMembers.Remove(removedMember);        
    }
}


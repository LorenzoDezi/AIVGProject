using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SquadDangerSensor : SquadSensor {

    public override void Init(SquadManager manager) {
        base.Init(manager);

        foreach(var squadMember in squadMembers) {
            squadMember.DangerFound += OnDangerFound;
        }
    }

    protected override void OnAddMember(SquadComponent newMember) {
        if (!squadMembers.Contains(newMember)) {
            squadMembers.Add(newMember);
            newMember.DangerFound += OnDangerFound;
        }
    }

    protected override void OnRemoveMember(SquadComponent removedMember) {
        base.OnRemoveMember(removedMember);
        removedMember.DangerFound -= OnDangerFound;
    }

    private void OnDangerFound(IDangerous danger) {
        foreach(var squadMember in squadMembers) {
            squadMember.RegisterDanger(danger);
        }
    }

}


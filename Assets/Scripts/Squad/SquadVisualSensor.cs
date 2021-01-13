using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SquadVisualSensor : SquadSensor {

    private List<SquadComponent> squadMembers;

    public override void Init(SquadManager manager) {

        base.Init(manager);

        this.squadMembers = manager.CurrSquadMembers;
        foreach(SquadComponent squadMember in squadMembers) {
            squadMember.EnemySpotted += OnEnemySpottedEvent;
        }

        manager.AddedMember += OnAddMember;
        manager.RemovedMember += OnRemoveMember;
    }

    private void OnAddMember(SquadComponent newMember) {
        if (!squadMembers.Contains(newMember)) {
            squadMembers.Add(newMember);
            newMember.EnemySpotted += OnEnemySpottedEvent;
        }
    }

    private void OnRemoveMember(SquadComponent removedMember) {
        squadMembers.Remove(removedMember);
        removedMember.EnemySpotted -= OnEnemySpottedEvent;
    }

    private void OnEnemySpottedEvent(Transform enemy) {
        foreach (var squadMember in squadMembers) {
            squadMember.Spotted(enemy);
        }
    }
}


using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SquadVisualSensor : SquadSensor {

    [SerializeField]
    private WorldStateKey enemySeenKey;
    private WorldState enemySeenWS;

    [SerializeField]
    private WorldStateKey enemyLostKey;
    private WorldState enemyLostWS;

    private List<SquadComponent> squadMembers;
    private bool spotted;
    private int lostCount;

    public override void Init(SquadManager manager) {

        base.Init(manager);

        enemySeenWS = new WorldState(enemySeenKey, false);
        if (manager.SquadPerception[enemySeenKey] == null)
            manager.SquadPerception.Add(enemySeenWS);

        enemyLostWS = new WorldState(enemyLostKey, false);
        if (manager.SquadPerception[enemyLostKey] == null)
            manager.SquadPerception.Add(enemyLostWS);

        this.squadMembers = manager.CurrSquadMembers;
        foreach(SquadComponent squadMember in squadMembers) {
            squadMember.EnemySpotted += OnEnemySpottedEvent;
            squadMember.EnemyLost += OnEnemyLost;
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
        if (spotted)
            lostCount--;
        else {
            spotted = true;
            foreach (var squadMember in squadMembers) {
                squadMember.Spotted(enemy);
            }
            UpdatePerception();
        }        
    }

    private void OnEnemyLost() {
        lostCount++;
        if(lostCount == squadMembers.Count) {
            lostCount = 0;
            spotted = false;
            UpdatePerception();
        }
    }

    private void UpdatePerception() {
        enemySeenWS.BoolValue = spotted;
        manager.SquadPerception[enemySeenKey].Update(enemySeenWS);
        enemyLostWS.BoolValue = !spotted;
        manager.SquadPerception[enemyLostKey].Update(enemyLostWS);
    }
}


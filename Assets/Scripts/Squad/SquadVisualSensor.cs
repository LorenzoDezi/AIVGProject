using GOAP;
using System;
using System.Collections;
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

    private Transform enemy;

    private List<SquadComponent> squadMembers;
    private bool spotted;
    private int lostCount;

    [SerializeField, Tooltip("Time needed for the squad to forget the enemy encounter (enemyLost = false)")]
    private float alertTime;
    private Coroutine alertTimer;

    private void Update() {
        if(spotted) {
            foreach(var member in squadMembers) {
                member.UpdateEnemy(enemy);
            }
        }
    }

    public override void Init(SquadManager manager) {

        base.Init(manager);

        InitPerception();

        this.squadMembers = manager.CurrSquadMembers;
        foreach (SquadComponent squadMember in squadMembers) {
            squadMember.EnemySpotted += OnEnemySpotted;
            squadMember.EnemyLost += OnEnemyLost;
        }

        manager.AddedMember += OnAddMember;
        manager.RemovedMember += OnRemoveMember;
    }

    private void OnAddMember(SquadComponent newMember) {
        if (!squadMembers.Contains(newMember)) {
            squadMembers.Add(newMember);
            newMember.EnemySpotted += OnEnemySpotted;
            newMember.EnemyLost += OnEnemyLost;
        }
    }

    private void OnRemoveMember(SquadComponent removedMember) {
        squadMembers.Remove(removedMember);
        removedMember.EnemySpotted -= OnEnemySpotted;
        removedMember.EnemyLost -= OnEnemyLost;
    }

    private void OnEnemySpotted(Transform enemy) {
        if (spotted)
            lostCount--;
        else {

            spotted = true;
            this.enemy = enemy;
            foreach (var squadMember in squadMembers) {
                squadMember.Spotted(enemy);
            }

            if(alertTimer != null) {
                StopCoroutine(alertTimer);
                alertTimer = null;
            }

            UpdatePerception();
        }        
    }

    private void OnEnemyLost() {
        lostCount++;
        if (lostCount == squadMembers.Count) {
            lostCount = 0;
            spotted = false;
            UpdatePerception();
            alertTimer = StartCoroutine(StartAlertTimer());
        }
    }

    private IEnumerator StartAlertTimer() {
        yield return new WaitForSeconds(alertTime);
        enemyLostWS.BoolValue = false;
        manager.SquadPerception[enemyLostKey].Update(enemyLostWS);
    }

    private void InitPerception() {
        enemySeenWS = new WorldState(enemySeenKey, false);
        if (manager.SquadPerception[enemySeenKey] == null)
            manager.SquadPerception.Add(enemySeenWS);

        enemyLostWS = new WorldState(enemyLostKey, false);
        if (manager.SquadPerception[enemyLostKey] == null)
            manager.SquadPerception.Add(enemyLostWS);
    }

    private void UpdatePerception() {
        enemySeenWS.BoolValue = spotted;
        manager.SquadPerception[enemySeenKey].Update(enemySeenWS);
        enemyLostWS.BoolValue = !spotted;
        manager.SquadPerception[enemyLostKey].Update(enemyLostWS);
    }
}


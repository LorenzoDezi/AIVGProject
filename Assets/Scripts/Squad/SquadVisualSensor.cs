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
    private Queue<Vector3> lastEnemyPositions;
    [SerializeField]
    private int enemyPosBufferMaxSize = 2;
    [SerializeField]
    private float timeToUpdateEnemyPos = 1.5f;
    private float currTimeToUpdateEnemyPos;

    private bool spotted;
    private int lostCount;

    private SearchCoordinator searchCoordinator;

    private void Awake() {
        searchCoordinator = GetComponent<SearchCoordinator>();
        lastEnemyPositions = new Queue<Vector3>();
    }

    #region override methods
    public override void Init(SquadManager manager) {

        base.Init(manager);
        foreach (SquadComponent squadMember in squadMembers) {
            squadMember.EnemySpotted += OnEnemySpotted;
            squadMember.EnemyLost += OnEnemyLost;
        }

        InitPerception();
    }

    protected override void OnAddMember(SquadComponent newMember) {
        if (!squadMembers.Contains(newMember)) {
            squadMembers.Add(newMember);
            newMember.EnemySpotted += OnEnemySpotted;
            newMember.EnemyLost += OnEnemyLost;
        }
    }

    protected override void OnRemoveMember(SquadComponent removedMember) {
        base.OnRemoveMember(removedMember);
        removedMember.EnemySpotted -= OnEnemySpotted;
        removedMember.EnemyLost -= OnEnemyLost;
    }
    #endregion


    private void Update() {
        if (spotted && currTimeToUpdateEnemyPos >= timeToUpdateEnemyPos) {

            currTimeToUpdateEnemyPos = 0f;

            foreach (var member in squadMembers) {
                member.UpdateLastSeen(enemy);
            }

            if (lastEnemyPositions.Count >= enemyPosBufferMaxSize)
                lastEnemyPositions.Dequeue();
            lastEnemyPositions.Enqueue(enemy.position);

        } else {
            currTimeToUpdateEnemyPos += Time.deltaTime;
        }
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

            currTimeToUpdateEnemyPos = timeToUpdateEnemyPos;

            UpdatePerception();
        }        
    }

    private void OnEnemyLost() {
        lostCount++;
        if (lostCount == squadMembers.Count) {
            lostCount = 0;
            spotted = false;
            UpdatePerception();
            if(lastEnemyPositions.Count > 0) {
                searchCoordinator?.SetupSearchPoints(lastEnemyPositions.Dequeue());
                lastEnemyPositions.Clear();
            }
        }
    }

    #region perception
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
    #endregion
}


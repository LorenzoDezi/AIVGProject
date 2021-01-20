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
    private Queue<Vector3> enemyPosBuffer;
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
        searchCoordinator.SearchTerminated += OnTerminateSearch;
        enemyPosBuffer = new Queue<Vector3>();
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

        if (!spotted)
            return;

        if (currTimeToUpdateEnemyPos >= timeToUpdateEnemyPos) {

            currTimeToUpdateEnemyPos = 0f;

            squadMembers.ForEach((member) => member.UpdateLastSeen(enemy));

            if (enemyPosBuffer.Count >= enemyPosBufferMaxSize)
                enemyPosBuffer.Dequeue();
            enemyPosBuffer.Enqueue(enemy.position);

        } else
            currTimeToUpdateEnemyPos += Time.deltaTime;
    }

    private void OnEnemySpotted(Transform enemy) {

        if (spotted)
            lostCount--;
        else {
            spotted = true;
            this.enemy = enemy;
            squadMembers.ForEach((member) => member.SpotEnemy(enemy));
            currTimeToUpdateEnemyPos = timeToUpdateEnemyPos;
            searchCoordinator.StopTimer();
            UpdatePerception();
        }        
    }

    private void OnTerminateSearch() {
        squadMembers.ForEach((member) => member[enemyLostKey].BoolValue = false);
        enemyLostWS.BoolValue = false;
    }

    private void OnEnemyLost() {
        lostCount++;
        if (lostCount == squadMembers.Count) {
            lostCount = 0;
            spotted = false;
            if(enemyPosBuffer.Count > 0) {
                searchCoordinator?.SetupSearchPoints(enemyPosBuffer.Dequeue());
                enemyPosBuffer.Clear();
            }
            //Stopping "single" enemy search
            squadMembers.ForEach((member) => member.StopSearch());
            UpdatePerception();
        }
    }

    #region perception
    private void InitPerception() {
        enemySeenWS = manager.SquadPerception[enemySeenKey]; 
        if (enemySeenWS == null) {
            enemySeenWS = new WorldState(enemySeenKey, false);
            manager.SquadPerception.Add(enemySeenWS);
        }

        enemyLostWS = manager.SquadPerception[enemyLostKey]; 
        if (enemyLostWS == null) {
            enemyLostWS = new WorldState(enemyLostKey, false);
            manager.SquadPerception.Add(enemyLostWS);
        }
    }

    private void UpdatePerception() {
        enemySeenWS.BoolValue = spotted;
        enemyLostWS.BoolValue = !spotted;
    } 
    #endregion
}


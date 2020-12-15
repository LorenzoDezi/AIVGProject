﻿using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class CoverSensor : MonoBehaviour {

    private new Transform transform;
    private Agent agentToUpdate;
    private NavigationComponent navComponent;

    [SerializeField]
    private WorldStateKey coverAvailableKey;
    private WorldState coverAvailableWSTracked;

    [SerializeField]
    private WorldStateKey inCoverKey;
    private WorldState inCoverWSTracked;

    [SerializeField]
    private LayerMask coverMask;
    private List<CoverComponent> coversAvailable;
    public CoverComponent BestCover { get; private set; }
    public UnityEvent BestCoverChanged;
    private CoverComponent currCover;
    public bool InCover => currCover == BestCover;


    [SerializeField]
    private float bestCoverCheckInterval = 1f;
    private Coroutine bestCoverCheckCoroutine;




    #region public methods
    public void GoInCover(CoverComponent cover) {
        currCover = cover;
        inCoverWSTracked.BoolValue = true;
        agentToUpdate.UpdatePerception(inCoverWSTracked);
        navComponent.PathStarted.AddListener(OutOfCover);
        Debug.LogWarningFormat("Go in cover {0} -> {1}", gameObject.name, cover.name);
    }
    #endregion

    #region monobehaviour methods
    protected void Awake() {

        agentToUpdate = GetComponent<Agent>();
        transform = GetComponent<Transform>();
        navComponent = GetComponent<NavigationComponent>();

        coversAvailable = new List<CoverComponent>();
        BestCoverChanged = new UnityEvent();

        coverAvailableWSTracked = new WorldState(coverAvailableKey, false);
        inCoverWSTracked = new WorldState(inCoverKey, false);
        agentToUpdate.UpdatePerception(inCoverWSTracked);
        agentToUpdate.UpdatePerception(coverAvailableWSTracked);
    }

    private void OnTriggerEnter2D(Collider2D collider) {

        if (!coverMask.ContainsLayer(collider.gameObject.layer))
            return;

        CoverComponent newCover = collider.GetComponent<CoverComponent>();
        coversAvailable.Add(newCover);

        if (bestCoverCheckCoroutine == null)
            bestCoverCheckCoroutine = StartCoroutine(BestCoverCheck());
    }

    private void OnTriggerExit2D(Collider2D collider) {

        if (!coverMask.ContainsLayer(collider.gameObject.layer))
            return;

        CoverComponent cover = collider.GetComponent<CoverComponent>();
        coversAvailable.Remove(cover);

        if (bestCoverCheckCoroutine != null && coversAvailable.Count == 0) {
            StopCoroutine(bestCoverCheckCoroutine);
            bestCoverCheckCoroutine = null;
        }
    } 
    #endregion

    #region coroutines
    private IEnumerator BestCoverCheck() {
        var wait = new WaitForSeconds(bestCoverCheckInterval);
        while (true) {
            UpdateBestCover();
            yield return wait;
        }
    }
    #endregion

    #region private methods

    private void OutOfCover() {
        currCover.IsOccupied = false;
        currCover = null;
        inCoverWSTracked.BoolValue = false;
        agentToUpdate.UpdatePerception(inCoverWSTracked);
        navComponent.PathStarted.RemoveListener(OutOfCover);
        Debug.LogWarningFormat("Out of cover {0}", gameObject.name);
    }

    private void UpdateBestCover() {

        CoverComponent newBestCover = GetBestCover();

        if (newBestCover == BestCover)
            return;

        BestCover = newBestCover;
        BestCoverChanged.Invoke();
        coverAvailableWSTracked.BoolValue = BestCover != null;
        agentToUpdate.UpdatePerception(coverAvailableWSTracked);
    }

    private CoverComponent GetBestCover() {
        if (currCover != null && currCover.CanCover)
            return currCover;
        float minSqrDist = Mathf.Infinity;
        CoverComponent newBestCover = null;
        foreach (CoverComponent cover in coversAvailable) {
            if (cover.IsAvailable) {
                float sqrDist = cover.transform.SqrDistance(transform);
                if (sqrDist < minSqrDist) {
                    minSqrDist = sqrDist;
                    newBestCover = cover;
                }
            }
        }
        return newBestCover;
    }
    #endregion
}


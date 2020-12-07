using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class CoverSensor : Sensor {

    private new Transform transform;

    [SerializeField]
    private LayerMask coverMask;
    private List<CoverComponent> coversAvailable;
    public CoverComponent BestCover { get; private set; }


    [SerializeField]
    private float bestCoverCheckInterval = 1f;
    private Coroutine bestCoverCheckCoroutine;

    [SerializeField]
    private WorldStateKey inCoverKey;
    private WorldState inCoverWorldState;

    private CoverComponent currCover;
    public CoverComponent CurrCover {
        get {
            return currCover;
        }
        set {
            if (currCover != null)
                currCover.IsOccupied = false;
            currCover = value;
            if(value != null)
                GoInCover();
        }
    }

    [SerializeField]
    private float inCoverCheckInterval = 1f;
    private float inCoverMaxDistance = 4f;
    private Coroutine inCoverCheckCoroutine;

    public UnityEvent BestCoverChanged;

    #region monobehaviour methods
    protected override void Awake() {

        base.Awake();

        coversAvailable = new List<CoverComponent>();
        BestCoverChanged = new UnityEvent();
        //TODO: keyToUpdate -> how to avoid this delirium
        currWorldStateTracked = new WorldState(keyToUpdate, false);
        inCoverWorldState = new WorldState(inCoverKey, false);

        transform = GetComponent<Transform>();
        agentToUpdate.UpdatePerception(currWorldStateTracked);
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

    private IEnumerator CheckStillInCover() {
        var wait = new WaitForSeconds(inCoverCheckInterval);
        float sqrInCoverMaxDistance = inCoverMaxDistance * inCoverMaxDistance;
        while (transform.SqrDistance(currCover.Transform) < sqrInCoverMaxDistance) {
            yield return wait;
        }
        inCoverCheckCoroutine = null;
        currCover.IsOccupied = false;
        currCover = null;
    } 
    #endregion

    #region private methods
    private void GoInCover() {
        currCover.IsOccupied = true;
        inCoverWorldState.BoolValue = true;
        agentToUpdate.UpdatePerception(inCoverWorldState);
        if (inCoverCheckCoroutine == null) {
            inCoverCheckCoroutine = StartCoroutine(CheckStillInCover());
        }
    }

    private void UpdateBestCover() {

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

        if (newBestCover == BestCover)
            return;

        BestCover = newBestCover;
        BestCoverChanged.Invoke();
        currWorldStateTracked.BoolValue = BestCover != null;
        agentToUpdate.UpdatePerception(currWorldStateTracked);

        //DEBUG
        if (BestCover != null)
            Debug.LogFormat("Best Cover is: {0} for {1}", BestCover.name, name);
        else
            Debug.LogFormat("No cover available for {0}", name);
    } 
    #endregion
}


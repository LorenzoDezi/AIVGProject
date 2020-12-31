using GOAP;
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
    private WorldStateKey inCoverKey;
    private WorldState inCoverWSTracked;

    [SerializeField]
    private LayerMask coverMask;
    private List<CoverComponent> coversAvailable;
    private CoverComponent currCover;



    #region public methods
    public void EnterCover(CoverComponent cover) {
        currCover = cover;
        currCover.CanCoverChangedEvent += OnCurrCanCoverChanged;
        UpdateInCoverWS(true);
        navComponent.PathStarted += OutOfCover;
        Debug.LogWarningFormat("Go in cover {0} -> {1}", gameObject.name, cover.name);
    }

    public CoverComponent GetBestCover() {
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

    public CoverComponent GetBestCoverInRange(float range, Transform target) {
        float minSqrDist = Mathf.Infinity;
        float rangeSqr = range * range;
        CoverComponent newBestCover = null;
        foreach (CoverComponent cover in coversAvailable) {
            if (cover.IsAvailable && cover.Transform.SqrDistance(target) <= rangeSqr) {
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

    #region monobehaviour methods
    protected void Awake() {

        agentToUpdate = GetComponent<Agent>();
        transform = GetComponent<Transform>();
        navComponent = GetComponent<NavigationComponent>();

        coversAvailable = new List<CoverComponent>();

        inCoverWSTracked = new WorldState(inCoverKey, false);
        agentToUpdate.UpdatePerception(inCoverWSTracked);
    }

    private void OnTriggerEnter2D(Collider2D collider) {

        if (!coverMask.ContainsLayer(collider.gameObject.layer))
            return;

        CoverComponent newCover = collider.GetComponent<CoverComponent>();
        coversAvailable.Add(newCover);
    }

    private void OnTriggerExit2D(Collider2D collider) {

        if (!coverMask.ContainsLayer(collider.gameObject.layer))
            return;

        CoverComponent cover = collider.GetComponent<CoverComponent>();
        coversAvailable.Remove(cover);
    } 
    #endregion

    #region private methods

    private void OutOfCover() {
        currCover.IsOccupied = false;
        currCover.CanCoverChangedEvent -= OnCurrCanCoverChanged;
        currCover = null;
        UpdateInCoverWS(false);
        navComponent.PathStarted -= OutOfCover;
    }

    private void UpdateInCoverWS(bool value) {
        inCoverWSTracked.BoolValue = value;
        agentToUpdate.UpdatePerception(inCoverWSTracked);
    }

    private void OnCurrCanCoverChanged(bool canCover) {
        if(!canCover) {
            OutOfCover();
        }
    }
    #endregion
}


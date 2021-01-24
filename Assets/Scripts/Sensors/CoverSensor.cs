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
    private List<CoverComponent> nearCovers;
    private CoverComponent currCover;



    #region public methods
    public void EnterCover(CoverComponent cover) {
        currCover = cover;
        currCover.CanCoverChangedEvent += OnCurrCanCoverChanged;
        navComponent.PathStarted += OutOfCover;
        inCoverWSTracked.BoolValue = true;
    }

    public CoverComponent GetBestCover() {

        if (currCover != null && currCover.CanCover)
            return currCover;

        float minSqrDist = Mathf.Infinity;
        CoverComponent newBestCover = null;
        foreach (CoverComponent cover in nearCovers) {
            if (cover.IsAvailable) {
                float sqrDist = cover.Transform.SqrDistance(transform);
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
        foreach (CoverComponent cover in nearCovers) {
            if (cover.IsAvailable && cover.Transform.SqrDistance(target) <= rangeSqr) {
                float sqrDist = cover.Transform.SqrDistance(transform);
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

        nearCovers = new List<CoverComponent>();

        inCoverWSTracked = agentToUpdate[inCoverKey];
        if(inCoverWSTracked == null) {
            inCoverWSTracked = new WorldState(inCoverKey, false);
            agentToUpdate.Add(inCoverWSTracked);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {

        if (!coverMask.ContainsLayer(collider.gameObject.layer))
            return;

        CoverComponent newCover = collider.GetComponent<CoverComponent>();
        nearCovers.Add(newCover);
    }

    private void OnTriggerExit2D(Collider2D collider) {

        if (!coverMask.ContainsLayer(collider.gameObject.layer))
            return;

        CoverComponent cover = collider.GetComponent<CoverComponent>();
        nearCovers.Remove(cover);
    } 
    #endregion

    #region private methods

    private void OutOfCover() {
        currCover.IsOccupied = false;
        currCover.CanCoverChangedEvent -= OnCurrCanCoverChanged;
        currCover = null;
        inCoverWSTracked.BoolValue = false;
        navComponent.PathStarted -= OutOfCover;
    }

    private void OnCurrCanCoverChanged(bool canCover) {
        if(!canCover) {
            OutOfCover();
        }
    }
    #endregion
}


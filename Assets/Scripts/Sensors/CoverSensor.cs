using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CoverSensor : Sensor {

    private new Transform transform;

    [SerializeField]
    private LayerMask coverMask;
    private CoverComponent bestCover;
    private List<CoverComponent> coversAvailable;

    [SerializeField]
    private float bestCoverCheckInterval = 1f;
    private Coroutine bestCoverCheckCoroutine;

    protected override void Awake() {

        base.Awake();

        coversAvailable = new List<CoverComponent>();
        currWorldStateTracked = new WorldState(keyToUpdate, false);

        transform = GetComponent<Transform>();
        agentToUpdate.UpdatePerception(currWorldStateTracked);
    }

    private void OnTriggerEnter2D(Collider2D collider) {

        if (!coverMask.ContainsLayer(collider.gameObject.layer))
            return;

        CoverComponent newCover = collider.GetComponent<CoverComponent>();
        coversAvailable.Add(newCover);

        if(bestCoverCheckCoroutine == null)
            bestCoverCheckCoroutine = StartCoroutine(BestCoverCheck());
    }

    private void OnTriggerExit2D(Collider2D collider) {

        if (!coverMask.ContainsLayer(collider.gameObject.layer))
            return;

        CoverComponent cover = collider.GetComponent<CoverComponent>();
        coversAvailable.Remove(cover);

        if(bestCoverCheckCoroutine != null && coversAvailable.Count == 0) {
            StopCoroutine(bestCoverCheckCoroutine);
            bestCoverCheckCoroutine = null;
        }
    }

    private IEnumerator BestCoverCheck() {
        var wait = new WaitForSeconds(bestCoverCheckInterval);
        while(true) {
            UpdateBestCover();
            yield return wait;
        }
    }

    private void UpdateBestCover() {

        float minSqrDist = Mathf.Infinity;
        bestCover = null;
        foreach (CoverComponent cover in coversAvailable) {
            if (cover.CanCover) {
                float sqrDist = cover.transform.SqrDistance(transform);
                if (sqrDist < minSqrDist) {
                    minSqrDist = sqrDist;
                    bestCover = cover;
                }
            }
        }

        currWorldStateTracked.BoolValue = bestCover != null;
        agentToUpdate.UpdatePerception(currWorldStateTracked);

        //DEBUG
        if (bestCover != null)
            Debug.LogFormat("Best Cover is: {0} for {1}", bestCover.name, name);
        else
            Debug.LogFormat("No cover available for {0}", name);
    }
}


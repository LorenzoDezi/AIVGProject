﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CoverComponent : MonoBehaviour
{
    private new Transform transform;
    public Transform Transform => transform;

    [SerializeField]
    private LayerMask toCoverFromMask;
    [SerializeField]
    private LayerMask obstacleMask;
    Transform transfToCoverFrom;
    bool hasTransformInSight;

    private float checkCoverInterval = 1f;
    private Coroutine checkCoverCoroutine;

    private bool canCover;
    public bool CanCover => canCover;
    public bool IsOccupied { get; set; }
    public bool IsAvailable => canCover && !IsOccupied;

    void Awake()
    {
        transform = GetComponent<Transform>();
    }

    IEnumerator CheckCover() {

        var wait = new WaitForSeconds(checkCoverInterval);
        while(hasTransformInSight) {
            canCover = transform.HasObstacleInBetween(transfToCoverFrom, obstacleMask);
            //if (CanCover)
            //    Debug.LogFormat("{0} can cover from player", gameObject.name);
            yield return wait;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {

        SetTransformInSight(other.gameObject, true);

        checkCoverCoroutine = StartCoroutine(CheckCover());

    }

    private void OnTriggerExit2D(Collider2D other) {

        SetTransformInSight(other.gameObject, false);

        if(checkCoverCoroutine != null) {
            StopCoroutine(checkCoverCoroutine);
            checkCoverCoroutine = null;
        }
    }

    

    private void SetTransformInSight(GameObject obj, bool inSight) {

        if (!toCoverFromMask.ContainsLayer(obj.layer))
            return;

        transfToCoverFrom = obj.transform;
        hasTransformInSight = inSight;
    }

}

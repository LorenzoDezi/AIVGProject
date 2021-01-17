using System;
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
    [SerializeField]
    private LayerMask wallMask;
    Transform enemyTransf;
    bool hasTransformInSight;

    private float checkCoverInterval = 1f;
    private Coroutine checkCoverCoroutine;

    private bool canCover;
    public bool CanCover => canCover;

    public delegate void CanCoverChangedHandler(bool canCover);
    public event CanCoverChangedHandler CanCoverChangedEvent;
    public bool IsOccupied { get; set; }
    public bool IsAvailable => canCover && !IsOccupied;

    void Awake()
    {
        transform = GetComponent<Transform>();
    }

    IEnumerator CheckCover() {

        var wait = new WaitForSeconds(checkCoverInterval);
        while(hasTransformInSight) {
            canCover = CanCoverFrom(enemyTransf.position);
            CanCoverChangedEvent?.Invoke(canCover);
            yield return wait;
        }
    }

    public bool CanCoverFrom(Vector3 position) {
        return Physics2D.Linecast(position, transform.position, obstacleMask) &&
            !Physics2D.Linecast(position, transform.position, wallMask);
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

        hasTransformInSight = inSight;
        if(inSight) {
            enemyTransf = obj.transform;
        }      
    }

}

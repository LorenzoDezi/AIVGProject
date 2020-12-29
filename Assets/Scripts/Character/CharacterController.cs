using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour {

    #region cached components
    private Rigidbody2D rigidBody;
    private Animator animator;
    private new Collider2D collider;
    private Vector2 movementDir;
    private Vector2 aimPosition;
    #endregion

    #region movement fields
    [SerializeField]
    private float movementSpeed = 50f;
    [SerializeField]
    private float dashSpeed = 10f;
    [SerializeField]
    private float dashLength = 1f;
    [SerializeField]
    private float dashReloadTime = 0.1f;
    private float currDashReloadTime;
    private bool isDashing;
    #endregion

    #region aiming fields
    [SerializeField]
    private float aimSpeedDegrees = 200f;
    public float AimSpeedDegrees {
        get => aimSpeedDegrees;
        set => aimSpeedDegrees = value;
    }

    private float angleToTarget;
    public float AngleToTarget => angleToTarget; 
    #endregion

    #region monobehaviour methods
    private void Awake() {

        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        collider = GetComponent<Collider2D>();

        currDashReloadTime = dashReloadTime;
    }

    private void FixedUpdate() {
        rigidBody.velocity = movementDir * (isDashing ? dashSpeed : movementSpeed);
        RotateTowardsAimPosition();
    }
    #endregion

    #region private methods
    private void RotateTowardsAimPosition() {

        Vector2 lookDir = (aimPosition - rigidBody.position).normalized;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.AngleAxis(angle, Vector3.forward),
            aimSpeedDegrees * Time.fixedDeltaTime
        );
        angleToTarget = rotation.eulerAngles.z;
        rigidBody.MoveRotation(angleToTarget);
    }

    private IEnumerator DashCountdown() {
        currDashReloadTime = 0f;
        yield return new WaitForSeconds(dashLength);
        isDashing = false;
        var wait = new WaitForEndOfFrame();
        while (currDashReloadTime < dashReloadTime) {
            currDashReloadTime += Time.deltaTime;
            yield return wait;
        }
    }
    #endregion

    #region public methods
    public void Move(Vector2 movementDir) {
        this.movementDir = movementDir;
    }

    public void Dash() {

        if (isDashing || currDashReloadTime < dashReloadTime)
            return;
        isDashing = true;
        StartCoroutine(DashCountdown());
    }

    public void AimAt(Vector3 worldPosition) {
        aimPosition = worldPosition;
    }

    public void OnDeath() {
        gameObject.layer = 0;
        animator.SetTrigger("Death");
        collider.enabled = false;
    } 
    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Animator animator;
    private new Collider2D collider;
    private Vector2 movementDir;
    private Vector2 aimPosition;

    [SerializeField]
    private float movementSpeed = 50f;
    [SerializeField]
    private float aimSpeedDegrees = 200f;
    public float AimSpeedDegrees {
        get => aimSpeedDegrees;
        set => aimSpeedDegrees = value;
    }

    private void Awake() {
        InitFields();
    }

    private void InitFields() {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        collider = GetComponent<Collider2D>();
    }

    private void FixedUpdate() {
        rigidBody.velocity = movementDir * movementSpeed;
        RotateTowardsAimPosition();
    }

    private void RotateTowardsAimPosition() {
        Vector2 lookDir = (aimPosition - rigidBody.position).normalized;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.AngleAxis(angle, Vector3.forward),
            aimSpeedDegrees * Time.fixedDeltaTime
            );
        rigidBody.MoveRotation(rotation.eulerAngles.z);
    }

    public void Move(Vector2 movementDir) {
        this.movementDir = movementDir;
    }

    public void AimAt(Vector3 worldPosition) {
        aimPosition = worldPosition;
    }

    public void OnDeath() {
        gameObject.layer = 0;
        animator.SetTrigger("Death");
        collider.enabled = false;
    }
}

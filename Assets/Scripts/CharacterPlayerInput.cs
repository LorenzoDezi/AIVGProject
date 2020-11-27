using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterPlayerInput : MonoBehaviour
{
    private PlayerInputAction inputAction;
    private CharacterController characterController;
    private CrosshairController crosshairController;
    private GunController gunController;
    private Camera mainCamera;

    private bool isShooting;

    private void Awake() {
        InitFields();
    }

    private void Start() {
        InitInput();
    }

    private void Update() {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector2 worldMousePosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        characterController.AimAt(worldMousePosition);
        crosshairController.MoveAt(mouseScreenPosition, mainCamera);
        if (isShooting)
            gunController.TryToShoot();
    }

    private void InitFields() {
        inputAction = new PlayerInputAction();
        characterController = GetComponent<CharacterController>();
        gunController = GetComponentInChildren<GunController>();
        crosshairController = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<CrosshairController>();
        mainCamera = Camera.main;
    }

    private void InitInput() {
        var playerActions = inputAction.Player;
        playerActions.Movement.started += OnMovement;
        playerActions.Movement.performed += OnMovement;
        playerActions.Movement.canceled += OnMovement;
        playerActions.Shoot.started += OnStartShooting;
        playerActions.Shoot.canceled += OnStopShooting;
        inputAction.Enable();
    }

    void OnMovement(InputAction.CallbackContext context) {
        Vector2 value = context.ReadValue<Vector2>();
        characterController.Move(value);
    }

    void OnStartShooting(InputAction.CallbackContext context) {
        isShooting = true;
    }

    void OnStopShooting(InputAction.CallbackContext context) {
        isShooting = false;
    }

    public void OnDeath() {
        inputAction.Disable();
        characterController.Move(Vector2.zero);
        Destroy(this);
    }
}

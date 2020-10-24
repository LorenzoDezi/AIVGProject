using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterPlayerInput : MonoBehaviour
{
    private PlayerInputAction inputAction;
    private CharacterController characterController;
    private Camera mainCamera;
    private Vector3 mouseScreenPosition;

    private void Awake() {
        InitFields();
    }

    private void Start() {
        InitInput();
    }

    private void Update() {
        characterController.AimAt(mainCamera.ScreenToWorldPoint(mouseScreenPosition));
    }

    private void InitFields() {
        inputAction = new PlayerInputAction();
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    private void InitInput() {
        var playerActions = inputAction.Player;
        playerActions.Movement.started += OnMovement;
        playerActions.Movement.performed += OnMovement;
        playerActions.Movement.canceled += OnMovement;
        playerActions.Aim.started += OnAim;
        playerActions.Aim.performed += OnAim;
        playerActions.Aim.canceled += OnAim;
        inputAction.Enable();
    }

    void OnMovement(InputAction.CallbackContext context) {
        Vector2 value = context.ReadValue<Vector2>();
        characterController.Move(value);
    }

    void OnAim(InputAction.CallbackContext context) {
        mouseScreenPosition = context.ReadValue<Vector2>();
    }
}

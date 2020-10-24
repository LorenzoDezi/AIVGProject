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
    private Camera mainCamera;
    private Vector3 worldMousePosition;

    private void Awake() {
        InitFields();
    }

    private void Start() {
        InitInput();
    }

    private void Update() {
        characterController.AimAt(worldMousePosition);
        crosshairController.MoveAt(worldMousePosition);
    }

    private void InitFields() {
        inputAction = new PlayerInputAction();
        characterController = GetComponent<CharacterController>();
        crosshairController = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<CrosshairController>();
        mainCamera = Camera.main;
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        worldMousePosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
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
        Vector2 mouseScreenPosition = context.ReadValue<Vector2>();
        worldMousePosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }
}

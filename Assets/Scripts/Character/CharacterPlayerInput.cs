using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterPlayerInput : MonoBehaviour
{
    private PlayerInputAction inputAction;
    private GrenadeController grenadeLauncher;
    private CharacterController characterController;
    private CrosshairController crosshairController;
    private GunController gunController;
    private KnifeController knifeController;
    private HealthComponent healthComponent;
    private Camera mainCamera;

    [SerializeField]
    private LayerMask healthStationLayerMask;

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
        healthComponent = GetComponent<HealthComponent>();
        grenadeLauncher = GetComponentInChildren<GrenadeController>();
        gunController = GetComponentInChildren<GunController>();
        knifeController = GetComponentInChildren<KnifeController>();
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
        playerActions.KnifeAttack.started += OnStartKnifeAttack;
        playerActions.KnifeAttack.canceled += OnStopKnifeAttack;
        playerActions.GrenadeLaunch.started += LaunchGrenade;
        playerActions.Reload.started += OnReload;
        playerActions.Dash.started += OnDash;
        playerActions.Interaction.started += OnInteraction;
        inputAction.Enable();
    }

    void OnMovement(InputAction.CallbackContext context) {
        Vector2 value = context.ReadValue<Vector2>();
        characterController.Move(value);
    }

    public void OnInteraction(InputAction.CallbackContext context) {
        var hsCollider = Physics2D.OverlapCircle(transform.position, 2f, healthStationLayerMask);
        if (hsCollider != null && hsCollider.GetComponent<HealthStation>().UseRefill()) {
            healthComponent.Restore(healthComponent.MaxHealth);
        }
    }

    void OnDash(InputAction.CallbackContext context) {
        characterController.Dash();
    }

    void OnStartShooting(InputAction.CallbackContext context) {
        isShooting = true;
    }

    void OnStartKnifeAttack(InputAction.CallbackContext context) {
        knifeController.IsAttacking = true;
        isShooting = false;
    }

    void LaunchGrenade(InputAction.CallbackContext context) {
        grenadeLauncher.LaunchAt(crosshairController.Transform.position);
    }

    void OnStopKnifeAttack(InputAction.CallbackContext context) {
        knifeController.IsAttacking = false;
    }

    void OnStopShooting(InputAction.CallbackContext context) {
        isShooting = false;
    }

    void OnReload(InputAction.CallbackContext context) {
        gunController.Reload();
    }

    public void OnDeath() {
        inputAction.Disable();
        characterController.Move(Vector2.zero);
        Destroy(this);
    }
}

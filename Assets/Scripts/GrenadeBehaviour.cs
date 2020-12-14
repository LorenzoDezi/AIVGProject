using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GrenadeBehaviour : MonoBehaviour, IDangerous
{
    private new Transform transform;
    private BulletSpawner spawner;
    private new CircleCollider2D collider;

    [Header("Transforms")]
    [SerializeField]
    private Transform rendererTransform;
    [SerializeField]
    private Renderer radiusRenderer;
    private Transform radiusRendererTransform;

    [Header("Damage")]
    [SerializeField]
    private LayerMask damageableLayerMask;
    [SerializeField]
    private LayerMask obstacleLayerMask;
    [SerializeField]
    private float damage = 100f;

    [Header("Parameters")]
    [SerializeField]
    private float linearSpeed = 2f;
    [SerializeField]
    private float rotationSpeed = 2f;

    [SerializeField]
    private float timeToExplode = 2f;
    [SerializeField]
    private float explosionRadius;

    public float DangerRadius => explosionRadius;
    public Vector3 DangerSource => transform.position;
    public DangerEndEvent DangerEnd { get; } = new DangerEndEvent();

    private Vector3 targetPosition;
    private bool hasTarget;
    private Vector3 dirTowardTarget;
    public Vector3 TargetPosition {
        get => targetPosition;
        set {
            targetPosition = value;
            dirTowardTarget = (targetPosition - transform.position).normalized;
            dirTowardTarget.z = 0f;
            hasTarget = true;
        }
    }


    private void Awake() {
        transform = GetComponent<Transform>();
        collider = GetComponent<CircleCollider2D>();
    }

    private void Start() {

        spawner = GameManager.BulletSpawner;
        collider.enabled = false;
        radiusRenderer.enabled = false;
        collider.radius = explosionRadius + 2f;

        Transform radiusTransform = radiusRenderer.transform;
        Vector3 localScale = radiusTransform.localScale;
        localScale.x = explosionRadius;
        localScale.y = explosionRadius;
        radiusTransform.localScale = localScale;
    }

    private void Update() {

        if(hasTarget) {

            if(Vector3.Distance(transform.position, targetPosition) <= 1f) {

                hasTarget = false;
                collider.enabled = true;
                radiusRenderer.enabled = true;
                StartCoroutine(ExplodeCoroutine());

            } else {

                transform.position += dirTowardTarget * linearSpeed * Time.deltaTime;
                rendererTransform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

            }          
        }
    }

    private IEnumerator ExplodeCoroutine() {
        yield return new WaitForSeconds(timeToExplode);
        DangerEnd.Invoke(this);
        radiusRenderer.enabled = false;
        collider.enabled = false;
        ApplyDamage();
        spawner.ReleaseBullet(gameObject);
    }

    private void ApplyDamage() {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position, explosionRadius, damageableLayerMask
        );

        foreach (var collider in colliders) {
            if (!collider.transform.HasObstacleInBetween(transform, obstacleLayerMask))
                collider.GetComponent<HealthComponent>().Damage(damage);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBehaviour : MonoBehaviour
{
    private new Transform transform;
    private BulletSpawner spawner;
    private new CircleCollider2D collider;

    [SerializeField]
    private Transform rendererTransform;
    [SerializeField]
    private Renderer radiusRenderer;
    private Transform radiusRendererTransform;

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

    [SerializeField]
    private float linearSpeed = 2f;
    [SerializeField]
    private float rotationSpeed = 2f;

    [SerializeField]
    private float timeToExplode = 2f;
    [SerializeField]
    private float explosionRadius;

    private void Awake() {
        transform = GetComponent<Transform>();
        collider = GetComponent<CircleCollider2D>();
    }

    private void Start() {

        spawner = GameManager.BulletSpawner;
        collider.enabled = false;
        radiusRenderer.enabled = false;
        collider.radius = explosionRadius;

        Transform radiusTransform = radiusRenderer.transform;
        Vector3 localScale = radiusTransform.localScale;
        localScale.x = explosionRadius;
        localScale.y = explosionRadius;
        radiusTransform.localScale = localScale;
    }

    

    void Update()
    {
        if(hasTarget) {
            if(Vector3.Distance(transform.position, targetPosition) <= 1f) {
                hasTarget = false;
                collider.enabled = true;
                radiusRenderer.enabled = true;
                StartCoroutine(ExplodeCoroutine());
                //TODO: Explosion countdown
            } else {
                transform.position += dirTowardTarget * linearSpeed * Time.deltaTime;
                rendererTransform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            }          
        }
    }

    private IEnumerator ExplodeCoroutine() {
        yield return new WaitForSeconds(timeToExplode);
        radiusRenderer.enabled = false;
        //TODO Damage
        collider.enabled = false;
        spawner.ReleaseBullet(gameObject);
    }
}

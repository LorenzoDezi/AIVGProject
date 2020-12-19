using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    private new Transform transform;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float ttl = 5f;
    private float currentTtl;
    [SerializeField]
    private float damage = 10f;
    [SerializeField]
    private float hitDistance = 1f;
    [SerializeField, Tooltip("The layers colliding with the bullet")]
    private LayerMask hitLayers;
    [SerializeField, Tooltip("The layer that the bullet can damage (Player => enemy and viceversa)")]
    private LayerMask damageHitLayer;
    private BulletSpawner spawner;

    private void Awake() {
        transform = GetComponent<Transform>();
        spawner = GameManager.BulletSpawner;
    }

    private void OnEnable() {
        currentTtl = 0f;
    }

    private void Update() {

        var hit = Physics2D.Raycast(transform.position, transform.right, hitDistance, hitLayers.value);

        if (hit) {
            TryToDamage(hit);
            Destroy();
            return;
        } else
            transform.position += transform.right * speed * Time.deltaTime;

        currentTtl += Time.deltaTime;
        if(currentTtl >= ttl) {
            Destroy();
        }
    }

    private void TryToDamage(RaycastHit2D hit) {
        GameObject gameObjectHit = hit.collider.gameObject;
        if (damageHitLayer.ContainsLayer(gameObjectHit.layer))
            gameObjectHit.GetComponent<HealthComponent>().Damage(damage);
    }

    private void Destroy() {
        spawner.ReleaseBullet(gameObject);
    }
}

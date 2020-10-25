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
    [SerializeField]
    private float damage = 10f;
    [SerializeField]
    private float hitDistance = 1f;
    [SerializeField, Tooltip("The layers colliding with the bullet")]
    private LayerMask hitLayers;
    [SerializeField, Tooltip("The layer that the bullet can damage (Player => enemy and viceversa)")]
    private LayerMask damageHitLayer;
    public BulletSpawner Spawner { get; set; }

    private void Awake() {
        transform = GetComponent<Transform>();
    }

    private void OnEnable() {
        //TODO: ttl reset
    }

    private void Update() {
        var hit = Physics2D.Raycast(transform.position, transform.right, hitDistance, hitLayers.value);
        if(hit) {
            TryToDamage(hit);
            Destroy();
        } else
            transform.position += transform.right * speed * Time.deltaTime;
        //TODO: ttl update
    }

    private void TryToDamage(RaycastHit2D hit) {
        GameObject gameObjectHit = hit.collider.gameObject;
        if (gameObjectHit.layer == damageHitLayer.value)
            gameObjectHit.GetComponent<IDamageable>()?.Damage(damage);
    }

    private void Destroy() {
        Spawner?.ReleaseBullet(gameObject);
    }
}

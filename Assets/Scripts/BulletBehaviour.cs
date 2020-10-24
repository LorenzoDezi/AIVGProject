using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    private new Transform transform;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float damage = 10f;
    [SerializeField]
    private float hitDistance = 1f;
    [SerializeField]
    private LayerMask hitLayers;
    [SerializeField]
    private string enemyHitLayerName = "Enemy";
    private int enemyHitLayer;

    private void Awake() {
        transform = GetComponent<Transform>();
        enemyHitLayer = LayerMask.NameToLayer(enemyHitLayerName);
    }

    private void Update() {
        var hit = Physics2D.Raycast(transform.position, transform.right, hitDistance, hitLayers.value);
        if(hit) {
            TryToDamage(hit);
            Destroy();
        } else
            transform.position += transform.right * speed * Time.deltaTime;
    }

    private void TryToDamage(RaycastHit2D hit) {
        GameObject gameObjectHit = hit.collider.gameObject;
        if (gameObjectHit.layer == enemyHitLayer)
            gameObjectHit.GetComponent<IDamageable>()?.Damage(damage);
    }

    private void Destroy() {
        gameObject.SetActive(false);
    }
}

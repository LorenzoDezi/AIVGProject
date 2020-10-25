using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private float lastShotTime;
    [SerializeField]
    private float shootInterval = 0.5f;

    [SerializeField]
    private Transform bulletSpawn;
    private BulletSpawner spawner;

    private void Start() {
        spawner = GameManager.BulletSpawner;
        lastShotTime = shootInterval;
    }

    public void TryToShoot() {
        if((Time.time - lastShotTime) >= shootInterval) {
            lastShotTime = Time.time;
            Shoot();
        }
    }

    private void Shoot() {
        var bullet = spawner.GetBullet(gameObject.tag);
        bullet.transform.position = bulletSpawn.transform.position;
        bullet.transform.right = bulletSpawn.transform.right;
    }

}

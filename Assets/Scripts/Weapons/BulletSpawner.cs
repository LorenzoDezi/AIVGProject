using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject playerBulletPrefab;
    [SerializeField]
    private GameObject enemyBulletPrefab;
    [SerializeField]
    private GameObject grenadePrefab;
    private Dictionary<string, Queue<GameObject>> bullets = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, GameObject> bulletPrefabs = new Dictionary<string, GameObject>();

    private void Start() {
        bullets.Add(GameManager.PlayerTag, new Queue<GameObject>());
        bullets.Add(GameManager.EnemyTag, new Queue<GameObject>());
        bullets.Add(grenadePrefab.tag, new Queue<GameObject>());
        bulletPrefabs.Add(GameManager.PlayerTag, playerBulletPrefab);
        bulletPrefabs.Add(GameManager.EnemyTag, enemyBulletPrefab);
        bulletPrefabs.Add(grenadePrefab.tag, grenadePrefab);
    }

    public GameObject GetBullet(string tag) {
        Queue<GameObject> bulletsPerTag = bullets[tag];
        GameObject bulletPrefab = bulletPrefabs[tag];
        if (bulletsPerTag.Count == 0) {
            var bulletInstance = GameObject.Instantiate(bulletPrefab);
            bullets[tag].Enqueue(bulletInstance);
        }
        var bullet = bulletsPerTag.Dequeue();
        bullet.SetActive(true);
        return bullet;
    }

    public GameObject GetGrenade() {
        return GetBullet(grenadePrefab.tag);
    }

    public void ReleaseBullet(GameObject bullet) {
        bullet.SetActive(false);
        bullets[bullet.tag].Enqueue(bullet);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject playerBulletPrefab;
    [SerializeField]
    private GameObject enemyBulletPrefab;
    private Dictionary<string, Queue<GameObject>> bullets = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, GameObject> bulletPrefabs = new Dictionary<string, GameObject>();

    private void Start() {
        bullets.Add(GameManager.PlayerTag, new Queue<GameObject>());
        bullets.Add(GameManager.EnemyTag, new Queue<GameObject>());
        bulletPrefabs.Add(GameManager.PlayerTag, playerBulletPrefab);
        bulletPrefabs.Add(GameManager.EnemyTag, enemyBulletPrefab);
    }

    public GameObject GetBullet(string tag) {
        Queue<GameObject> bulletsPerTag = bullets[tag];
        GameObject bulletPrefab = bulletPrefabs[tag];
        if (bulletsPerTag.Count == 0) {
            var bulletInstance = GameObject.Instantiate(bulletPrefab);
            AddBullet(bulletInstance, tag);
        }
        var bullet = bulletsPerTag.Dequeue();
        bullet.SetActive(true);
        return bullet;
    }

    public void ReleaseBullet(GameObject bullet) {
        bullet.SetActive(false);
        bullets[bullet.tag].Enqueue(bullet);
    }

    private void AddBullet(GameObject bullet, string tag) {
        var bulletBehaviour = bullet.GetComponent<BulletBehaviour>();
        bulletBehaviour.Spawner = this;
        bullets[tag].Enqueue(bullet);
    }



}

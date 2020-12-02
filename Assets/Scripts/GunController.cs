using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private float lastShotTime;
    [SerializeField]
    private float shootInterval = 0.5f;
    [SerializeField]
    private int maxShotsPerClip = 5;
    private int currentShotsInClip;

    [SerializeField]
    private float timeToReload = 1f;
    [SerializeField]
    private SpriteRenderer reloadSprite;
    private float currentReloadTime;
    private Coroutine reloadCoroutine;

    public bool HasShotsInClip => currentShotsInClip > 0;

    [SerializeField]
    private Transform bulletSpawn;
    private BulletSpawner spawner;

    private void Start() {
        spawner = GameManager.BulletSpawner;
        lastShotTime = shootInterval;
        currentShotsInClip = maxShotsPerClip;
        reloadSprite.enabled = false;
    }

    public void TryToShoot() {
        if((Time.time - lastShotTime) >= shootInterval && HasShotsInClip) {
            lastShotTime = Time.time;
            Shoot();
            currentShotsInClip--;
        }
    }

    public void Reload() {
        if(reloadCoroutine == null && currentShotsInClip != maxShotsPerClip)
            reloadCoroutine = StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine() {
        reloadSprite.enabled = true;
        yield return new WaitForSeconds(timeToReload);
        currentShotsInClip = maxShotsPerClip;
        reloadSprite.enabled = false;
        reloadCoroutine = null;
    }

    private void Shoot() {
        var bullet = spawner.GetBullet(gameObject.tag);
        bullet.transform.position = bulletSpawn.position;
        bullet.transform.right = bulletSpawn.right;
    }

    

}

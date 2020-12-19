using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    [SerializeField]
    private SpriteRenderer gunSprite;

    [SerializeField]
    private Transform bulletSpawn;
    private BulletSpawner spawner;

    public UnityEvent Reloaded = new UnityEvent();
    public UnityEvent EmptyClip = new UnityEvent();
    public bool HasShotsInClip => currentShotsInClip > 0;

    private void Start() {
        spawner = GameManager.BulletSpawner;
        lastShotTime = shootInterval;
        currentShotsInClip = maxShotsPerClip;
        reloadSprite.enabled = false;
    }

    public void UseWeapon(bool value) {
        gunSprite.enabled = value;
    }

    public void TryToShoot() {
        if((Time.time - lastShotTime) >= shootInterval && HasShotsInClip) {
            lastShotTime = Time.time;
            Shoot();
            currentShotsInClip--;
            if (currentShotsInClip == 0)
                EmptyClip.Invoke();
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
        Reloaded.Invoke();
        reloadCoroutine = null;
    }

    private void Shoot() {
        var bullet = spawner.GetBullet(gameObject.tag);
        bullet.transform.position = bulletSpawn.position;
        bullet.transform.right = bulletSpawn.right;
    }

    

}

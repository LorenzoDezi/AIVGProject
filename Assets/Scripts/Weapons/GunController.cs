using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunController : WeaponController
{
    private float lastShotTime;
    [SerializeField]
    private float shootInterval = 0.5f;

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

    public bool HasShotsInClip => currAmmo > 0;

    public delegate void GunLoadHandler(bool isLoaded);
    public event GunLoadHandler GunLoadStatusChanged;

    protected override void Start() {
        base.Start();
        lastShotTime = shootInterval;
        reloadSprite.enabled = false;
    }

    public void UseWeapon(bool value) {
        gunSprite.enabled = value;
    }

    public void TryToShoot() {
        if((Time.time - lastShotTime) >= shootInterval && HasShotsInClip) {
            lastShotTime = Time.time;
            Shoot();
            currAmmo--;
            RaiseAmmoChangedEvent(currAmmo);
            if (currAmmo == 0)
                GunLoadStatusChanged?.Invoke(false);
        }
    }

    public void Reload() {
        if(reloadCoroutine == null && currAmmo != maxAmmo)
            reloadCoroutine = StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine() {
        reloadSprite.enabled = true;
        yield return new WaitForSeconds(timeToReload);
        currAmmo = maxAmmo;
        RaiseAmmoChangedEvent(currAmmo);
        reloadSprite.enabled = false;
        GunLoadStatusChanged?.Invoke(true);
        reloadCoroutine = null;
    }

    private void Shoot() {
        var bullet = spawner.GetBullet(gameObject.tag);
        bullet.transform.position = bulletSpawn.position;
        bullet.transform.right = bulletSpawn.right;
    }
}

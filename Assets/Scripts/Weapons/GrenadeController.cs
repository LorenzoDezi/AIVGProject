using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public delegate void DangerFoundHandler(IDangerous danger);

public class GrenadeController : WeaponController {

    private new Transform transform;

    public int NeededAmmo => maxAmmo - currAmmo;

    [SerializeField]
    private float timeToReload = 2f;
    private float currReloadTime = 0f;

    [SerializeField]
    private float maxHitDistance = 10f;
    private float maxHitDistanceSqr;
    public float WeaponRange => maxHitDistance;

    private bool canLaunch;

    public event DangerFoundHandler GrenadeLaunched;

    private void Awake() {
        transform = GetComponent<Transform>();
    }

    protected override void Start() {
        base.Start();
        maxHitDistanceSqr = maxHitDistance * maxHitDistance;
        canLaunch = true;
    }

    private IEnumerator ReloadCoroutine() {
        var wait = new WaitForSeconds(timeToReload);
        yield return wait;
        canLaunch = true;
    }

    public void LaunchAt(Vector3 position) {
        if(currAmmo > 0 && canLaunch && (position - transform.position).sqrMagnitude <= maxHitDistanceSqr) {
            var grenade = spawner.GetGrenade();
            grenade.transform.position = transform.position;
            var behaviour = grenade.GetComponent<GrenadeBehaviour>();
            behaviour.TargetPosition = position;
            GrenadeLaunched?.Invoke(behaviour);
            canLaunch = false;
            currAmmo--;
            RaiseAmmoChangedEvent(currAmmo);
            StartCoroutine(ReloadCoroutine());
        }
    }

    public void ReloadAmmo(int grenades) {
        currAmmo += grenades;
        if (currAmmo > maxAmmo)
            currAmmo = maxAmmo;
        RaiseAmmoChangedEvent(currAmmo);
    }
}


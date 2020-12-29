using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GrenadeController : MonoBehaviour {

    private BulletSpawner spawner;
    private new Transform transform;

    [SerializeField]
    private float timeToReload = 2f;
    private float currReloadTime = 0f;

    [SerializeField]
    private float maxHitDistance = 10f;
    private float maxHitDistanceSqr;
    public float WeaponRange => maxHitDistance;

    private bool canLaunch;

    private void Awake() {
        transform = GetComponent<Transform>();
    }

    private void Start() {
        spawner = GameManager.BulletSpawner;
        maxHitDistanceSqr = maxHitDistance * maxHitDistance;
        canLaunch = true;
    }

    private IEnumerator ReloadCoroutine() {
        var wait = new WaitForSeconds(timeToReload);
        yield return wait;
        canLaunch = true;
    }

    public void LaunchAt(Vector3 position) {
        if(canLaunch && (position - transform.position).sqrMagnitude <= maxHitDistanceSqr) {
            var grenade = spawner.GetGrenade();
            grenade.transform.position = transform.position;
            grenade.GetComponent<GrenadeBehaviour>().TargetPosition = position;
            canLaunch = false;
            StartCoroutine(ReloadCoroutine());
        }
    }
}


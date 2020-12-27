using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SniperController : MonoBehaviour {

    private LineRenderer sniperLaserRenderer;
    private Material sniperLaserMat;
    private SpriteRenderer sniperRenderer;
    private new Transform transform;

    [SerializeField]
    private LayerMask hitLayerMask;
    [SerializeField]
    private LayerMask enemyLayerMask;

    [SerializeField]
    private float maxHitDistance = 10f;
    [SerializeField]
    private float damage = 50f;

    [SerializeField]
    private float timeToShoot = 2f;
    private float currTimeToShoot;

    [SerializeField]
    private float timeToReload = 2f;
    private float currTimeToReload;
    private bool isReloading;

    private Vector3[] linePositions;

    private bool isUsing;
    public bool IsShooting { get; set; }

    public void SetUsing(bool isUsing) {
        this.isUsing = isUsing;
        sniperRenderer.enabled = isUsing;
        this.enabled = isUsing;
        if (isUsing)
            SetLaserPositions();
        sniperLaserRenderer.enabled = isUsing;
    }

    private void Awake() {
        InitFields();

        linePositions = new Vector3[2];
        sniperLaserRenderer.positionCount = 2;
        SetLaserPositions();
        SetUsing(false);
        sniperLaserMat = sniperLaserRenderer.material;
        currTimeToReload = timeToReload;
    }

    private void InitFields() {
        transform = GetComponent<Transform>();
        sniperLaserRenderer = GetComponentInChildren<LineRenderer>();
        sniperRenderer = GetComponentInChildren<SpriteRenderer>();        
    }

    private void SetLaserPositions() {
        var sortingOrder = sniperRenderer.sortingOrder + 1;
        linePositions[0] = transform.position;
        linePositions[0].z = sortingOrder;
        linePositions[1] = transform.position + transform.right * maxHitDistance;
        linePositions[1].z = sortingOrder;
        sniperLaserRenderer.SetPositions(linePositions);
    }

    private void Update() {
        SetLaserPositions();
        UpdateShootingState();
    }

    private void UpdateShootingState() {
        if(isReloading)
            UpdateReload();
        else if (IsShooting)
            LoadSniperShot();
    }

    private void LoadSniperShot() {

        var color = sniperLaserMat.color;

        if (isShotObstructed()) {
            currTimeToShoot = 0f;
            currTimeToReload = 0f;
            color.a = 0f;

        } else if (currTimeToShoot <= timeToShoot) {
            color.a = currTimeToShoot / timeToShoot;
            currTimeToShoot += Time.deltaTime;

        } else {
            currTimeToShoot = 0f;
            currTimeToReload = 0f;
            color.a = 0f;
            isReloading = true;
            Shoot();

        }

        sniperLaserMat.color = color;
    }

    private void UpdateReload() {
        if (currTimeToReload < timeToReload) {
            currTimeToReload += Time.deltaTime;
        } else {
            isReloading = false;
            sniperLaserRenderer.enabled = true;
        }
    }

    private bool isShotObstructed() {
        var hit = Physics2D.Raycast(transform.position, transform.right, maxHitDistance, hitLayerMask);
        if (hit) {
            var hitGameObject = hit.collider.gameObject;
            return !enemyLayerMask.ContainsLayer(hitGameObject.layer);
        }
        return false;
    }

    private void Shoot() {
        var hit = Physics2D.Raycast(transform.position, transform.right, maxHitDistance, hitLayerMask);
        if(hit) {
            var hitGameObject = hit.collider.gameObject;
            if(enemyLayerMask.ContainsLayer(hitGameObject.layer))
                hitGameObject.GetComponent<HealthComponent>().Damage(damage);
        }
    }
}


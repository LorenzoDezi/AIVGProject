using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public delegate void AmmoChangeHandler(int currAmmo);

public class WeaponController : MonoBehaviour {

    [SerializeField]
    protected int maxAmmo = 3;
    protected int currAmmo;

    protected BulletSpawner spawner;

    public event AmmoChangeHandler AmmoChanged;

    protected void RaiseAmmoChangedEvent(int currAmmo) {
        AmmoChanged?.Invoke(currAmmo);
    }

    protected virtual void Start() {
        spawner = GameManager.BulletSpawner;
        currAmmo = maxAmmo;
        RaiseAmmoChangedEvent(currAmmo);
    }

}


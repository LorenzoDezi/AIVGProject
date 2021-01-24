using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GunSensor : MonoBehaviour {

    private Agent agent;
    [SerializeField]
    private WorldStateKey weaponLoadedKey;
    private WorldState weaponLoadedWSTracked;

    protected void Awake() {

        agent = GetComponent<Agent>();

        weaponLoadedWSTracked = agent[weaponLoadedKey];
        if(weaponLoadedWSTracked == null) {
            weaponLoadedWSTracked = new WorldState(weaponLoadedKey, true);
            agent.Add(weaponLoadedWSTracked);
        }

        var gunController = GetComponentInChildren<GunController>();
        gunController.GunLoadStatusChanged += (isLoaded) => UpdatePerception(isLoaded);
    }

    private void UpdatePerception(bool value) {
        weaponLoadedWSTracked.BoolValue = value;
    }
}


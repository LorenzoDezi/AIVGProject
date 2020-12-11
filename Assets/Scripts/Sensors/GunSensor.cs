using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GunSensor : MonoBehaviour {

    private Agent agentToUpdate;
    [SerializeField]
    private WorldStateKey weaponLoadedKey;
    private WorldState weaponLoadedWSTracked;

    protected void Awake() {

        agentToUpdate = GetComponent<Agent>();

        weaponLoadedWSTracked = new WorldState(weaponLoadedKey, true);
        agentToUpdate.UpdatePerception(weaponLoadedWSTracked);

        var gunController = GetComponentInChildren<GunController>();
        gunController?.EmptyClip.AddListener(() => UpdatePerception(false));
        gunController?.Reloaded.AddListener(() => UpdatePerception(true));
    }

    private void UpdatePerception(bool value) {
        weaponLoadedWSTracked.BoolValue = value;
        agentToUpdate.UpdatePerception(weaponLoadedWSTracked);
    }
}


using GOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GunSensor : Sensor {

    protected override void Awake() {
        base.Awake();
        currWorldStateTracked = new WorldState(keyToUpdate, true);
        agentToUpdate.UpdatePerception(currWorldStateTracked);
        var gunController = GetComponentInChildren<GunController>();
        gunController?.EmptyClip.AddListener(() => UpdatePerception(false));
        gunController?.Reloaded.AddListener(() => UpdatePerception(true));
    }

    private void UpdatePerception(bool value) {
        currWorldStateTracked.BoolValue = value;
        agentToUpdate.UpdatePerception(currWorldStateTracked);
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class SquadSensor : MonoBehaviour {

    protected SquadManager manager;

    public virtual void Init(SquadManager manager) {
        this.manager = manager;
    }

}


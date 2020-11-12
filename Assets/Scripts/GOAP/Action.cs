using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP {

    public abstract class Action : ScriptableObject {
        [SerializeField]
        private List<WorldState> preconditions;
        [SerializeField]
        private List<WorldState> effects;
        [SerializeField]
        private int cost;

        //TODO
    }

}



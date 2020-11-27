using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class ExtensionMethods {

    public static bool ContainsLayer(this LayerMask layerMask, int layer) {
        return layerMask == (layerMask | (1 << layer));
    }
}


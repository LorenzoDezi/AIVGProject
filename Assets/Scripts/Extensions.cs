using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtensions {

    public static bool ContainsLayer(this LayerMask layerMask, int layer) {
        return layerMask == (layerMask | (1 << layer));
    }
}

public static class Math2D {

    public static Vector2 RotatedBy(this Vector2 original, float angle) {
        //result is for caching purpose
        Vector2 result = new Vector2();
        result.x = original.x * Mathf.Cos(angle * Mathf.Deg2Rad) + original.y * -Mathf.Sin(angle * Mathf.Deg2Rad);
        result.y = original.x * Mathf.Sin(angle * Mathf.Deg2Rad) + original.y * Mathf.Cos(angle * Mathf.Deg2Rad);
        return result;

    }
}

public static class TransformExtensions {

    public static bool HasObstacleInBetween(this Transform from, Transform to, LayerMask obstacleLayerMask) {
        return Physics2D.Linecast(from.position, to.position, obstacleLayerMask);
    }

    public static float SqrDistance(this Transform start, Transform end) {
        return (end.position - start.position).sqrMagnitude;
    }
}

public static class DictionaryExtensions {

    public static T UpdateWith<T, K, V>(this T original, IDictionary<K, V> update) where T : Dictionary<K, V> {
        foreach(var pair in update) {
            if (original.ContainsKey(pair.Key))
                original[pair.Key] = pair.Value;
            else
                original.Add(pair.Key, pair.Value);
        }
        return original;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformUtils {
    public static Vector3 GetPositionXZ(this Transform transform) {
        var pos = transform.position;
        return new Vector3(pos.x, 0, pos.z);
    }
}

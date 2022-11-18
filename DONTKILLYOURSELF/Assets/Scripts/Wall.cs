using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, IDeflector
{
    public Vector2 getNormal()
    {
        //Vector2 tnorm = new Vector2(0, -1);
        float newAngle = (270 + transform.eulerAngles.z) * Mathf.PI / 180;
        Vector2 normal = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
        return normal;
    }
}

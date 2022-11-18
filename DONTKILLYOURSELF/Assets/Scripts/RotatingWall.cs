using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingWall : Wall
{
    private float angleOffset;
    private float prevOffset;
    [SerializeField]private float offsetMin;
    [SerializeField]private float offsetMax;
    [SerializeField]private float speed;
    private int frameCounter;
    [SerializeField]private bool flip;

    void Awake()
    {
        angleOffset = 0;
        prevOffset = 0;
    }

    void FixedUpdate()
    {
        frameCounter++;
        float t = frameCounter * speed;
        if(flip)
            t *= -1;
        float halfRange = (offsetMax - offsetMin) / 2;
        angleOffset = offsetMin + halfRange + Mathf.Sin(t) * halfRange;
        float angle = transform.eulerAngles.z - prevOffset;
        transform.eulerAngles = new Vector3(0, 0, angle + angleOffset);
        prevOffset = angleOffset;
    }
}

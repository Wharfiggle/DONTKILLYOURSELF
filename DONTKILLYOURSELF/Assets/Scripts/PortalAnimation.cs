using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalAnimation : MonoBehaviour
{
    private float spinSpeedOriginal;
    [SerializeField] float spinSpeed;
    [SerializeField] float scaleSpeed;
    [SerializeField] float scaleAmmount;
    private int frameCounter;
    [SerializeField] private bool flip;
    [SerializeField] private int spinSpeedUpTime;
    [SerializeField] private float spinSpeedUpAmmount;
    private int spinSpeedUpFrames;

    void Awake()
    {
        frameCounter = 0;
        spinSpeedUpFrames = 0;
        spinSpeedOriginal = spinSpeed;
    }

    void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + spinSpeed);

        float min = 1 - scaleAmmount;
        float max = 1 + scaleAmmount;
        frameCounter++;
        float t = frameCounter * scaleSpeed;
        if(flip)
            t *= -1;
        float halfRange = (max - min) / 2;
        float scale = min + halfRange + Mathf.Sin(t) * halfRange;
        float scale2 = min + halfRange + Mathf.Sin(t * 1.2f) * halfRange;
        transform.localScale = new Vector3(scale, scale2, 0);

        if(spinSpeedUpFrames > 0)
        {
            spinSpeedUpFrames--;
            t = (spinSpeedUpTime - spinSpeedUpFrames) / (float)spinSpeedUpTime;
            if(t <= 0.5)
                spinSpeed = spinSpeedOriginal + spinSpeedUpAmmount * t * 2;
            else
                spinSpeed = spinSpeedOriginal + spinSpeedUpAmmount * (1 - t) * 2;
        }
    }

    public void speedUp()
    {
        spinSpeedUpFrames = spinSpeedUpTime;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField]float speed; // 1 = 1 full rotation every second
    float frameCounter;

    void FixedUpdate()
    {
        float inc = 360f / 50 * speed;
        transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z + inc);
    }
}

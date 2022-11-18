using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballBounceParticle : MonoBehaviour
{
    [SerializeField] private int time;
    private int frames;

    void Awake()
    {
        frames = time;
    }

    void FixedUpdate()
    {
        frames--;
        if(frames == 0)
            Destroy(gameObject);
    }
}

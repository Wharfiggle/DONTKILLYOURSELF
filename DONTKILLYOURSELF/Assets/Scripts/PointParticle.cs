using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointParticle : MonoBehaviour
{
    private int pointAmmount;
    private bool shot;
    private TextMeshPro text;
    [SerializeField] private int dissipateTime;
    private int dissipateFrames;
    [SerializeField] private float speed;

    void Awake()
    {
        pointAmmount = 0;
        shot = false;
        text = GetComponent<TextMeshPro>();
        text.enabled = false;
        dissipateFrames = dissipateTime;
    }

    void FixedUpdate()
    {
        if(shot)
        {
            dissipateFrames--;
            if(dissipateFrames == 0)
                Destroy(gameObject);
            float t = (dissipateTime - dissipateFrames) / (float)dissipateTime;
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1 - t);
            transform.position = new Vector3(transform.position.x, transform.position.y + speed, transform.position.z);
        }
    }

    public void shoot(int p)
    {
        pointAmmount = p;
        shot = true;
        text.enabled = true;
        text.text = "+" + pointAmmount.ToString();
    }
}

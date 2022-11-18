using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Fireball, IDeflectable, IDeflector
{
    [SerializeField] private int dieTime;
    private int dieFrames;
    private SpriteRenderer sprite;
    [SerializeField] private AudioClip dieSound;

    new protected void Awake()
    {
        base.Awake();
        dieFrames = 0;
        sprite = GetComponentInChildren<SpriteRenderer>();
        /*int origDieTime = dieTime;
        dieTime += Random.Range((int)(-dieTime * 1), (int)(dieTime * 1) + 1);
        animate.speed = dieTime / (float)origDieTime;
        Debug.Log(animate.speed);*/
    }

    new protected void FixedUpdate()
    {
        base.FixedUpdate();
        if(dieFrames > 0)
        {
            dieFrames--;
            float t = (dieTime - dieFrames) / (float)dieTime;
            //exponential interpolation doesn't work from high to low or when 0 is involved, so we eerp from 1 to 2 then correct it after
            float eerp = Mathf.Pow(1f, 1f - t) * Mathf.Pow(2f, t);
            eerp = (eerp - 1); // 0 to 1
            //double eerp
            eerp = Mathf.Pow(1f, 1f - eerp) * Mathf.Pow(1.5f, eerp);
            eerp = 1 - (eerp - 1); // 1 to 0.5
            //Debug.Log(eerp);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, eerp);
            if(dieFrames == 0)
                Destroy(gameObject);
        }
    }

    public bool hurt()
    {
        if(dieFrames == 0)
        {
            dieFrames = dieTime;
            animate.SetInteger("anim", 1);
            active = false;
            sound.PlayOneShot(dieSound);
            return true;
        }
        else
            return false;
    }
}

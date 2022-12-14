using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Fireball, IDeflectable, IDeflector
{
    [SerializeField] private int dieTime;
    private int dieFrames;
    [SerializeField] private AudioClip dieSound;
    private Transform player;
    [SerializeField] private float fbSpeed;
    [SerializeField] private int fbSpawnTime;
    private int fbSpawnFrames;
    [SerializeField] private GameObject fireball;
    [SerializeField] private Color chargeColor;
    [SerializeField] private int chargeReleaseTime;
    private Color origColor;

    new protected void Awake()
    {
        base.Awake();
        dieFrames = 0;
        /*int origDieTime = dieTime;
        dieTime += Random.Range((int)(-dieTime * 1), (int)(dieTime * 1) + 1);
        animate.speed = dieTime / (float)origDieTime;
        Debug.Log(animate.speed);*/
        player = GameObject.FindGameObjectWithTag("Player").transform;
        fbSpawnFrames = fbSpawnTime;
        origColor = sprite.color;
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
        else
        {
            fbSpawnFrames--;
            if(fbSpawnFrames < chargeReleaseTime)
            {
                float t = (chargeReleaseTime - fbSpawnFrames) / (float)chargeReleaseTime;
                t *= t;
                sprite.color = new Color(chargeColor.r + (origColor.r - chargeColor.r) * t, chargeColor.g + (origColor.g - chargeColor.g) * t, chargeColor.b + (origColor.b - chargeColor.b) * t, chargeColor.a + (origColor.a - chargeColor.a) * t);
            }
            else
            {
                float t = ((fbSpawnTime - fbSpawnFrames) - chargeReleaseTime) / (float)(fbSpawnTime - chargeReleaseTime);
                t *= t;
                sprite.color = new Color(origColor.r + (chargeColor.r - origColor.r) * t, origColor.g + (chargeColor.g - origColor.g) * t, origColor.b + (chargeColor.b - origColor.b) * t, origColor.a + (chargeColor.a - origColor.a) * t);
            }
            if(fbSpawnFrames == 0)
            {
                Vector2 meToPlayer = new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y);
                float shootAngle = getAngleFromVector(meToPlayer);

                GameObject fbInstance = Instantiate(fireball, new Vector3(transform.position.x, transform.position.y, transform.position.z + 1), Quaternion.identity);
                IDeflectable fbScript = fbInstance.GetComponent<IDeflectable>();
                if(fbScript != null)
                    fbScript.shoot(shootAngle, fbSpeed, new Vector2(velx, vely));
                fbSpawnFrames = fbSpawnTime;
            }
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

    public override void deflect(float angle)
    {
        base.deflect(angle);
        /*Vector2 meToPlayer = new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y);
        float newAngle = getAngleFromVector(meToPlayer);
        this.angle = newAngle;*/
    }
}

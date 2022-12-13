using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Fireball : MonoBehaviour, IDeflectable, IDeflector
{
    protected float velx;
    protected float vely;
    private Vector2 initialVel;
    protected float angle; //RADIANS
    private float speed;
    [SerializeField] private float speedDec;
    private bool shot;
    private Vector2 startScale;
    protected bool active;
    [SerializeField] private float fizzleSpeedThreshold;
    [SerializeField] private LayerMask layer;
    protected Animator animate;
    private CircleCollider2D collide;
    [SerializeField] private GameObject bounceParticle;
    [SerializeField] private GameObject pointParticle;
    [SerializeField] private bool visuallyRotates;
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private AudioClip scoreSound1;
    [SerializeField] private AudioClip scoreSound2;
    [SerializeField] private AudioClip scoreSound3;
    [SerializeField] private AudioClip shootSound;
    //[SerializeField] private AudioClip burningSound;
    //[SerializeField] private AudioClip fizzleSound;
    protected AudioSource sound;
    struct Deflection
    {
        public IDeflector deflector;
        public Vector2 normal;
        public GameObject gameObject;
        public bool empty;
        public Deflection(IDeflector d, Vector2 n, GameObject go)
        {
            deflector = d;
            normal = n;
            gameObject = go;
            empty = false;
        }
        public Deflection(bool fuckyou)
        {
            deflector = null;
            normal = Vector2.zero;
            gameObject = null;
            empty = true;
        }
    }
    Deflection deflection;
    List<GameObject> collideIgnore;
    [SerializeField] public bool deflectsOffProjectiles;
    private TextMeshProUGUI pointsUI;
    private int pointModifier;
    [SerializeField] private bool hitsEnemies;
    [SerializeField] protected SpriteRenderer sprite;

    protected void Awake()
    {
        angle = 0;
        velx = 0;
        vely = 0;
        shot = false;
        startScale = new Vector2(transform.localScale.x, transform.localScale.y);
        active = true;
        deflection = new Deflection(false);
        collideIgnore = new List<GameObject>();
        animate = GetComponentInChildren<Animator>();
        collide = GetComponent<CircleCollider2D>();
        pointModifier = 1;
        pointsUI = GameObject.FindGameObjectWithTag("Points").GetComponent<TextMeshProUGUI>();
        sound = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected void FixedUpdate()
    {
        if(pointsUI == null)
            pointsUI = GameObject.FindGameObjectWithTag("Points").GetComponent<TextMeshProUGUI>();
        if(shot)
        {
            //velx = Mathf.Cos(angle) * speed + initialVel.x;
            //vely = Mathf.Sin(angle) * speed + initialVel.y;
            velx = Mathf.Cos(angle) * speed;
            vely = Mathf.Sin(angle) * speed;
            float scale = 1;
            if(fizzleSpeedThreshold > 0)
                scale = Mathf.Min(fizzleSpeedThreshold, speed) / fizzleSpeedThreshold;
            if(scale < 1 && active)
            {
                active = false;
                animate.SetInteger("anim", 1);
                //sound.PlayOneShot(fizzleSound);
            }
            transform.localScale = new Vector3(startScale.x * scale, startScale.y * scale, 0);
            speed -= speedDec;
            if(speed <= 0)
                Destroy(gameObject);

            //Deflect
            if(!deflection.empty)
            {
                //Debug.Log("ATTEMPT DEFLECT");
                Vector2 normal = deflection.deflector.getNormal();
                if(normal == Vector2.zero)
                    normal = deflection.normal;
                float normalAngle = getAngleFromVector(normal);
                //Debug.Log("normal found: " + normal);
                //check if projectile is relatively facing the deflector's normal, else flip normal
                if(!angleWithinRange(angle, normalAngle + Mathf.PI / 2, normalAngle + Mathf.PI * 3 / 2))
                    normalAngle += Mathf.PI;
                Vector2 newVel = Quaternion.AngleAxis(180, normal) * new Vector2(velx, vely) * -1;
                float newAngle = getAngleFromVector(newVel);
                deflect(newAngle);
                if(active && deflection.gameObject != null)
                {
                    //deflect other projectiles
                    IDeflectable deflectable = deflection.gameObject.GetComponent<IDeflectable>();
                    if(deflectable != null && deflectable.getDeflectsOffProjectiles())
                        deflectable.deflect(angle + Mathf.PI);
                }
                deflection = new Deflection(false);
                if(bounceSound != null)
                    sound.PlayOneShot(bounceSound);
            }

            //Collision check
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            //Vector2 startPoint = new Vector2(transform.position.x + Mathf.Cos(angle) * collide.radius, transform.position.y + Mathf.Sin(angle) * collide.radius);
            RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, collide.radius, direction, speed / 50, layer);
            bool done = false;
            List<bool> found = new List<bool>(collideIgnore.Count);
            for(int i = 0; i < collideIgnore.Count; i++)
                found.Add(false);
            for(int i = 0; i < hit.Length; i++)
            {
                if(hit[i].transform.gameObject != gameObject)
                {
                    int ind = collideIgnore.IndexOf(hit[i].transform.gameObject);
                    if(ind == -1 && !done)
                    {
                        IDeflectable deflectable = hit[i].transform.gameObject.GetComponent<IDeflectable>();
                        if(deflectable == null || getDeflectsOffProjectiles())
                        {
                            //Debug.Log("CIRCLECAST FOUND SOMETHING");
                            Enemy enemy = hit[i].transform.gameObject.GetComponent<Enemy>();
                            if(active)
                            {
                                PlayerHealth pHealth = hit[i].transform.gameObject.GetComponent<PlayerHealth>();
                                if(pHealth != null)
                                    pHealth.hurt();
                                if(enemy != null && hitsEnemies)
                                {
                                    bool valid = enemy.hurt();
                                    if(valid)
                                    {
                                        GameObject pointInstance = Instantiate(pointParticle, transform.position, Quaternion.identity);
                                        PointParticle pp = pointInstance.GetComponent<PointParticle>();
                                        pp.shoot(10 * pointModifier);
                                        GlobalVars.setScore(GlobalVars.getScore() + 10 * pointModifier);
                                        pointsUI.text = "Score: " + GlobalVars.getScore();
                                        if(scoreSound1 != null)
                                        {
                                            if(pointModifier == 1)
                                                sound.PlayOneShot(scoreSound1);
                                            else if(pointModifier == 2)
                                                sound.PlayOneShot(scoreSound2);
                                            else
                                                sound.PlayOneShot(scoreSound3);
                                        }
                                        pointModifier *= 2;
                                    }
                                }
                            }
                            float dist = Mathf.Sqrt(Mathf.Pow(hit[i].point.x - transform.position.x, 2) + Mathf.Pow(hit[i].point.y - transform.position.y, 2)) - collide.radius;
                            vely = Mathf.Cos(dist);
                            velx = Mathf.Sin(dist);
                            IDeflector deflector = hit[i].transform.gameObject.GetComponent<IDeflector>();
                            if(deflector != null && enemy == null)
                                deflection = new Deflection(deflector, hit[i].normal, hit[i].transform.gameObject);
                            collideIgnore.Add(hit[i].transform.gameObject);
                            done = true;
                        }
                    }
                    else if(ind != -1)
                        found[ind] = true;
                }
            }
            int removed = 0;
            for(int i = 0; i < found.Count; i++)
            {
                if(found[i] == false)
                {
                    collideIgnore.RemoveAt(i - removed);
                    removed++;
                }
            }
            found.Clear();
            transform.position = new Vector3(transform.position.x + (velx / 50), transform.position.y + (vely / 50), transform.position.z);

            //dont go out of bounds
            float halfWid = 9.8f - collide.radius;
            float halfHei = 5f - collide.radius;
            Vector2 defVec = new Vector2(0, 0);
            if(transform.position.x <= -halfWid && velx < 0)
                defVec.x = 1;
            else if(transform.position.x >= halfWid && velx > 0)
                defVec.x = -1;
            else if(transform.position.y >= halfHei && vely > 0)
                defVec.y = -1;
            else if(transform.position.y <= -halfHei && vely < 0)
                defVec.y = 1;
            if(defVec.x != 0 || defVec.y != 0)
                deflect(getAngleFromVector(defVec));
        }
    }

    public void shoot(float angle, float speed, Vector2 initialVel)
    {
        shot = true;
        this.angle = angle;
        this.speed = speed;
        this.initialVel = initialVel;
        if(shootSound != null)
            sound.PlayOneShot(shootSound);
        if(visuallyRotates)
            transform.eulerAngles = new Vector3(0, 0, angle * 180 / Mathf.PI - 90);
        //Debug.Log("a: " + angle * 180 / Mathf.PI);
    }

    public bool getDeflectsOffProjectiles()
    {
        return deflectsOffProjectiles;
    }

    public virtual void deflect(float angle)
    {
        if(shot)
        {
            initialVel = Vector2.zero;
            speed -= speedDec;
            this.angle = angle;
            velx = Mathf.Cos(angle) * speed;
            vely = Mathf.Sin(angle) * speed;
            if(visuallyRotates)
                transform.eulerAngles = new Vector3(0, 0, angle * 180 / Mathf.PI - 90);
            if(bounceParticle != null)
                Instantiate(bounceParticle, transform.position, transform.rotation);
        }
    }

    public Vector2 getNormal()
    {
        return Vector2.zero;
    }

    public float getAngleFromVector(Vector2 v)
    {
        float a = Mathf.Atan2(v.y, v.x);
        if(v.y == 0)
        {
            if(v.x > 0)
                a = 0;
            else
                a = Mathf.PI;
        }
        else if(v.x == 0)
        {
            if(v.y > 0)
                a = 1.5f * Mathf.PI;
            else
                a = 0.5f * Mathf.PI;
        }
        return a;
    }

    public bool angleWithinRange(float ang, float min, float max)
    {
        ang = ang * 180 / Mathf.PI;
        min = min * 180 / Mathf.PI;
        max = max * 180 / Mathf.PI;
        ang %= (360);
        min %= (360);
        max %= (360);
        if(ang < 0)
            ang = 360 + ang;
        if(min < 0)
            min = 360 + min;
        if(max < 0)
            max = 360 + max;
        if(min > max)
            return (min < ang || ang < max);
        else
            return (min < ang && ang < max);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        collideIgnore.Remove(collision.gameObject);
    }

    /*public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision != null && shot)
        {
            IDeflector deflector = collision.gameObject.GetComponent<IDeflector>();
            if(deflector != null)
            {
                //Debug.Log("ATTEMPT DEFLECT");
                Vector2 normal = deflector.getNormal();
                if(normal == Vector2.zero)
                    normal = collision.GetContact(0).normal;
                float normalAngle = getAngleFromVector(normal);
                //Debug.Log("normal found: " + normal);
                //check if projectile is aimed across the deflector's normal
                if(!angleWithinRange(angle, normalAngle + Mathf.PI / 2, normalAngle + Mathf.PI * 3 / 2))
                    normalAngle += Mathf.PI;
                Vector2 newVel = Quaternion.AngleAxis(180, normal) * new Vector2(velx, vely) * -1;
                float newAngle = getAngleFromVector(newVel);
                deflect(newAngle);
                IDeflectable deflectable = collision.gameObject.GetComponent<IDeflectable>();
                if(deflectable != null)
                    deflectable.deflect(angle + Mathf.PI);
                //else
                    //Debug.Log("did not deflect because of incompatible angles, min: " + (normalAngle + Mathf.PI / 2) * 180 / Mathf.PI + ", max: " + (normalAngle + Mathf.PI * 3 / 2) * 180 / Mathf.PI + ", angle: " + angle * 180 / Mathf.PI);
            }
        }
    }*/
}

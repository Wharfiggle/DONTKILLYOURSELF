using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D body;
    private float angle; //DEGREES
    private float velx;
    private float vely;
    private Animator animate;
    [SerializeField]private float speed;
    [SerializeField]private float speedInc;
    //in Input Manager: Horizontal and Vertical Sensitivity should be set to 1 - ((speed / speedInc) / 50)
    [SerializeField]private float stopInc;
    //in Input Manager: Horizontal and Vertical Gravity should be set to (speed / stopInc) / 50
    [SerializeField]private GameObject fireball;
    [SerializeField]private int fbSpawnTime;
    private int fbSpawnFrames;
    private Transform fbSpawnPoint;
    [SerializeField]private float fbSpeed;
    private Vector2 mousePos;
    private bool stickLooking;
    [SerializeField] int fuseTime;
    [SerializeField] private AudioClip fuseSound;
    private AudioSource sound;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        angle = -90;
        velx = 0;
        vely = 0;
        animate = GetComponent<Animator>();
        fbSpawnFrames = fbSpawnTime;
        fbSpawnPoint = transform.Find("fbSpawnPoint");
        mousePos = new Vector2(0, 0);
        stickLooking = false;
        sound = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {   
        if(Input.GetAxis("Vertical") != 0)
        {
            Debug.Log("left vertical: " + Input.GetAxis("Vertical"));
        }
        if(Input.GetAxis("Horizontal") != 0)
        {
            Debug.Log("left horizontal: " + Input.GetAxis("Horizontal"));
        }
        
        if(Input.GetAxis("RightVertical") != 0)
        {
            Debug.Log("right vertical: " + Input.GetAxis("RightVertical"));
        }
        if(Input.GetAxis("RightHorizontal") != 0)
        {
            Debug.Log("right horizontal: " + Input.GetAxis("RightHorizontal"));
        }

        //Rotate to follow mouse
        Vector2 rightStickVector = new Vector2(Input.GetAxis("RightHorizontal"), Input.GetAxis("RightVertical"));
        if(rightStickVector.x == 0 && rightStickVector.y == 0)
        {
            Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(!stickLooking || currentMousePos != mousePos)
            {
                stickLooking = false;
                mousePos = currentMousePos;
                angle = 180 / Mathf.PI * Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) - 90;
                transform.localEulerAngles = new Vector3(0, 0, angle);
            }
        }
        else //Rotate to follow right stick
        {
            stickLooking = true;
            angle = 180 / Mathf.PI * getAngleFromVector(rightStickVector) - 90;
            transform.localEulerAngles = new Vector3(0, 0, angle);
        }

        //Weighty WASD movement
        velx = body.velocity.x;
        vely = body.velocity.y;
        Vector2 leftStickVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if(leftStickVector.x == 0 && leftStickVector.y == 0)
        {
            Vector2 move = new Vector2(0, 0);
            if(Input.GetKey(KeyCode.W))
                move.y += 1;
            if(Input.GetKey(KeyCode.A))
                move.x -= 1;
            if(Input.GetKey(KeyCode.S))
                move.y -= 1;
            if(Input.GetKey(KeyCode.D))
                move.x += 1;
            float diagSpeed = Mathf.Cos(45f * Mathf.PI / 180f) * speed;
            float diagSpeedInc = diagSpeed / speed * speedInc;
            //Debug.Log("diagspeed: " + diagSpeed);
            //Debug.Log("speed: " + speed);
            float incx = speedInc;
            float incy = speedInc;
            float limx = speed;
            float limy = speed;
            if(move.x != 0 && move.y != 0)
            {
                incx = diagSpeedInc;
                incy = diagSpeedInc;
                limx = diagSpeed;
                limy = diagSpeed;
            }
            if(move.x == 0 && velx != 0)
            {
                incx = stopInc;
                if(velx > 0)
                    move.x = -1;
                else
                    move.x = 1;
                limx = 0;
            }
            if(move.y == 0 && vely != 0)
            {
                incy = stopInc;
                if(vely > 0)
                    move.y = -1;
                else
                    move.y = 1;
                limy = 0;
            }
            velx += incx * move.x;
            vely += incy * move.y;
            if(move.x == -1 && velx < -limx)
                velx = -limx;
            if(move.x == 1 && velx > limx)
                velx = limx;
            if(move.y == -1 && vely < -limy)
                vely = -limy;
            if(move.y == 1 && vely > limy)
                vely = limy;
        }
        else //weightiness taken care of in Input Manager
        {
            velx = speed * leftStickVector.x;
            vely = speed * leftStickVector.y;
        }

        body.velocity = new Vector3(velx, vely, 0);

        //Set speed in animator
        float a = Mathf.Atan2(vely, velx);
        float spd = vely / Mathf.Sin(a);
        if(velx < 0.001 && velx > -0.001 && vely < 0.001 && vely > -0.001)
            spd = 0;
        else if(velx < 0.001 && velx > -0.001)
            spd = vely;
        else if(vely < 0.001 && vely > -0.001)
            spd = velx;
        spd = Mathf.Abs(spd);
        animate.SetFloat("speed", spd);
        //Debug.Log(spd);

        //Spawn fireballs
        if(fbSpawnFrames > 0)
        {
            fbSpawnFrames--;
            if(fbSpawnFrames == fuseTime)
                sound.PlayOneShot(fuseSound);
            if(fbSpawnFrames == 0)
            {
                GameObject fbInstance = Instantiate(fireball, fbSpawnPoint.position, fbSpawnPoint.rotation);
                IDeflectable fbScript = fbInstance.GetComponent<IDeflectable>();
                if(fbScript != null)
                    fbScript.shoot((angle + 90) * Mathf.PI / 180f, fbSpeed, new Vector2(velx, vely));
                fbSpawnFrames = fbSpawnTime;
            }
        }
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
                a = 0.5f * Mathf.PI;
            else
                a = 1.5f * Mathf.PI;
        }
        return a;
    }
}

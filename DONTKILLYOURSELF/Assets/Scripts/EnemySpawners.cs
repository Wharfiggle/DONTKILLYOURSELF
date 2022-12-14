using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawners : MonoBehaviour
{
    private Transform[] spawners;
    private PortalAnimation[] portals;
    [SerializeField] private int spawnTime;
    private int spawnFrames;
    [SerializeField] private GameObject enemy;
    [SerializeField] private float enemySpeed;
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private float doubleSpawnChance;
    [SerializeField] private int startDelay;
    private AudioSource sound;

    void Awake()
    {
        spawnFrames = spawnTime + startDelay;
        Transform[] tempTransforms = gameObject.GetComponentsInChildren<Transform>();
        spawners = new Transform[tempTransforms.Length - 5];
        portals = new PortalAnimation[spawners.Length];
        int ammount = 0;
        for(int i = 0; i < tempTransforms.Length; i++)
        {
            SpriteRenderer stupidfuckingbullshit = tempTransforms[i].gameObject.GetComponent<SpriteRenderer>();
            if(tempTransforms[i] != transform && stupidfuckingbullshit == null)
            {
                spawners[ammount] = tempTransforms[i];
                portals[ammount] = tempTransforms[i].gameObject.GetComponentInChildren<PortalAnimation>();
                ammount++; 
            }
        }
        sound = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        spawnFrames--;
        if(spawnFrames == 0)
        {
            spawnFrames = spawnTime;
            spawn(-1);
        }
    }

    void spawn(int prev)
    {
        int rn = Random.Range(0, spawners.Length);
        if(prev == -1)
        {
            int doubleSpawn = Random.Range(0, 100);
            if(doubleSpawn < 100 * doubleSpawnChance)
                spawn(rn);
        }
        else
        {
            while(rn == prev && spawners.Length != 1)
                rn = Random.Range(0, spawners.Length);
        }
        Vector3 spawnPosition = new Vector3(spawners[rn].position.x, spawners[rn].position.y, spawners[rn].position.z + 1f);
        GameObject enemyInstance = Instantiate(enemy, spawnPosition, transform.rotation);
        IDeflectable enemyScript = enemyInstance.GetComponent<IDeflectable>();
        if(enemyScript != null)
            enemyScript.shoot((spawners[rn].localEulerAngles.z - 90) * Mathf.PI / 180, enemySpeed, Vector2.zero);
        portals[rn].speedUp();
        sound.PlayOneShot(spawnSound);
    }
}

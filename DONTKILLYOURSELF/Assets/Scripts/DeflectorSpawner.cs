using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeflectorSpawner : MonoBehaviour
{
    [SerializeField] private int deflectorSpawnTime;
    private int deflectorSpawnFrames;
    [SerializeField] private int deflectorTime;
    private int deflectorFrames;
    [SerializeField] private int transitionTime;
    private int transitionFrames;
    private enum DeflectorTypes {spinner, walls, both, END};
    DeflectorTypes mode;
    GameObject[] deflectors;
    List<GameObject> activeDeflectors;
    private bool reverse;

    void Awake()
    {
        deflectorSpawnFrames = deflectorSpawnTime;
        deflectorFrames = 0;
        transitionFrames = 0;
        mode = DeflectorTypes.both;
        deflectors = new GameObject[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            deflectors[i] = transform.GetChild(i).gameObject;
        }
        activeDeflectors = new List<GameObject>();
        reverse = false;
    }

    void FixedUpdate()
    {
        if(deflectorSpawnFrames > 0)
        {
            deflectorSpawnFrames--;
            if(deflectorSpawnFrames == 0)
            {
                mode = (DeflectorTypes)Random.Range(0, (int)DeflectorTypes.END);
                //Debug.Log("mode:" + mode);
                for(int i = 0; i < deflectors.Length; i++)
                {
                    string defName = deflectors[i].name;
                    if((mode == DeflectorTypes.walls || mode == DeflectorTypes.both) && (defName == "Right" || defName == "Left" || defName == "Top" || defName == "Bottom"))
                        activeDeflectors.Add(deflectors[i].transform.Find("Deflector").gameObject);
                    if((mode == DeflectorTypes.spinner || mode == DeflectorTypes.both) && defName == "Spinner")
                        activeDeflectors.Add(deflectors[i].transform.Find("Deflector").gameObject);
                }
                transitionFrames = transitionTime;
                reverse = false;
            }
        }
        else if(transitionFrames > 0)
        {
            transitionFrames--;
            float t = (transitionTime - transitionFrames) / (float)transitionTime;
            if(!reverse)
                t = Mathf.Sqrt(t);
            else
            {
                t = 1f - t;
                t = Mathf.Sqrt(t);
            }
            for(int i = 0; i < activeDeflectors.Count; i++)
            {
                Vector3 targ = activeDeflectors[i].transform.parent.Find("Position").position;
                Vector3 pos = activeDeflectors[i].transform.parent.Find("OffPosition").position;
                activeDeflectors[i].transform.position = new Vector3(pos.x + (targ.x - pos.x) * t, pos.y + (targ.y - pos.y) * t, pos.z);
            }
            if(transitionFrames == 0 && !reverse)
                deflectorFrames = deflectorTime;
            else if(transitionFrames == 0)
                deflectorSpawnFrames = deflectorSpawnTime;
        }
        else if(deflectorFrames > 0)
        {
            deflectorFrames--;
            if(deflectorFrames == 0)
            {
                transitionFrames = transitionTime;
                reverse = true;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballHurt : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            PlayerHealth ph = collider.gameObject.GetComponent<PlayerHealth>();
            if(ph != null)
                ph.hurt();
        }
    }
}

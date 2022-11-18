using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int invincTime;
    private int invincFrames;
    [SerializeField] private int invincFlashPeriod;
    [SerializeField] private int invincFlashLength;
    [SerializeField] private Color hurtColor;
    [SerializeField] private bool godmode;
    TextMeshProUGUI ui;
    private SpriteRenderer sprite;
    [SerializeField] private AudioClip hurtSound;
    private AudioSource sound;

    void Awake()
    {
        GameObject uiObj = GameObject.FindGameObjectWithTag("Health");
        if(uiObj != null)
            ui = uiObj.GetComponent<TextMeshProUGUI>();
        invincFrames = 0;
        sprite = gameObject.transform.parent.GetComponent<SpriteRenderer>();
        sound = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if(invincFrames > 0)
        {
            invincFrames--;
            if(invincFrames != 0)
            {
                if((invincTime - invincFrames) % invincFlashPeriod < invincFlashLength && invincFrames != 0)
                    sprite.color = new Color(hurtColor.r, hurtColor.g, hurtColor.b, 0.5f);
                else
                    sprite.color = new Color(hurtColor.r, hurtColor.g, hurtColor.b, 0.9f);
            }
            else
                sprite.color = new Color(1, 1, 1, 1);
        }

        if(ui != null)
        {
            string s = "HP: " + health;
            ui.SetText(s);
        }
        else
        {
            GameObject uiObj = GameObject.FindGameObjectWithTag("Health");
            if(uiObj != null)
                ui = uiObj.GetComponent<TextMeshProUGUI>();
        }
    }

    public void hurt()
    {
        if(invincFrames == 0)
        {
            if(!godmode)
                health--;
            invincFrames = invincTime;
            if(health <= 0)
                SceneManager.LoadScene("Lose", LoadSceneMode.Single);
            sound.PlayOneShot(hurtSound);
        }
    }
}

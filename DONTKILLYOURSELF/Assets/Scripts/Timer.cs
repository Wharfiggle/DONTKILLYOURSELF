using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    private float time;
    private int hours;
    private int minutes;
    private int seconds;
    private int milliseconds;
    private TextMeshProUGUI text;

    void Awake()
    {
        time = 0;
        text = GetComponent<TextMeshProUGUI>();
    }

    void FixedUpdate()
    {
        time += Time.deltaTime;
        hours = (int)time / (60 * 60);
        minutes = (int)(time % (60 * 60)) / 60;
        seconds = (int)time % 60;
        milliseconds = (int)((time % 1) * 100);
        text.text = minutes + ":";
        if(hours != 0)
            text.text = hours + ":" + minutes.ToString("D2") + ":";
        text.text += seconds.ToString("D2") + "." + milliseconds.ToString("D2");
        
        GlobalVars.setTime(time);
        GlobalVars.setTimeText(text.text);
    }
}

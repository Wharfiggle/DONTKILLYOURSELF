using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeDisplay : MonoBehaviour
{
    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = "TIME: \t" + GlobalVars.getTimeText() + "\nSCORE:\t" + GlobalVars.getScore();
    }
}

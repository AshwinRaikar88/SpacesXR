using UnityEngine;
using TMPro;
using System;

public class ClockDisplay : MonoBehaviour
{    
    public TMP_Text timeText;

    private string lastDisplayedMinute;

    void Start()
    {
        if (timeText == null)
        {
            Debug.LogError("ClockDisplay: TMP Text is not assigned.");
            return;
        }

        UpdateTimeText(); // Initial display
    }

    void Update()
    {
        string currentMinute = DateTime.Now.ToString("MMM dd, HH:mm");

        if (currentMinute != lastDisplayedMinute)
        {
            UpdateTimeText();
        }
    }

    void UpdateTimeText()
    {
        DateTime now = DateTime.Now;
        lastDisplayedMinute = now.ToString("MMM dd, HH:mm");
        timeText.text = "Time: " + lastDisplayedMinute;
    }
}

using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class StopWatch : MonoBehaviour
{
    private bool timerActive;
    private float currentTime;

    [SerializeField] private TMP_Text text;

    void Start()
    {
        currentTime = 0;
    }

    void Update()
    {
        
        if (timerActive)
        {
             currentTime = currentTime + Time.deltaTime;
        }
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        text.text = time.Minutes.ToString("00") + ":" + time.Seconds.ToString("00") + ":" + time.Milliseconds.ToString("000");
        
    }

    public void StartTimer()
    {
       
            timerActive = true;
            
    }

    public void StopTimer()
    {
            timerActive = false;
            RecordTimer();
        
    }
    
    
    public void RecordTimer()
    {
        
    }
    
}

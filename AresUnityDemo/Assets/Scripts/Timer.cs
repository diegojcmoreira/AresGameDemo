using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float timeRemaining;

    // Update is called once per frame
    void Update() {
        if (timeRemaining > 0) {
            
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        }
        
    }

    void DisplayTime(float timeToDisplay) {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);  
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        Debug.Log(string.Format("{0:00}:{1:00}", minutes, seconds));

    }
}

using System.Collections;
using UnityEngine;
using TMPro; 
public class TimerUI : MonoBehaviour
{
    public float timeRemaining = 120f; // Initial time
    public bool isTimerRunning = false;
    public TextMeshProUGUI timerText; // TMP component for UI

    void Start()
    {
        isTimerRunning = true;
    }

    void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerUI();
            }
            else
            {
                timeRemaining = 0;
                isTimerRunning = false;
                TimerFinished();
            }
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60); // Minute
            int seconds = Mathf.FloorToInt(timeRemaining % 60); // Remaining Time

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // "MM:SS" format
        }
    }

    void TimerFinished()
    {
        Debug.Log("Süre doldu!");
        timerText.text = "00:00"; // Disploay 00:00 when done
    }
}

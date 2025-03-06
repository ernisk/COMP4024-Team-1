using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Needed to load scenes

public class TimerUI : MonoBehaviour
{
    public float timeRemaining = 120f; // Initial time in seconds
    public bool isTimerRunning = false;
    public TextMeshProUGUI timerText; // TMP component for UI display

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
            int minutes = Mathf.FloorToInt(timeRemaining / 60); // Calculate minutes
            int seconds = Mathf.FloorToInt(timeRemaining % 60); // Calculate seconds

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Format "MM:SS"
        }
    }

    void TimerFinished()
    {
        Debug.Log("Time's up!");
        timerText.text = "00:00"; // Display 00:00 when time is up
        SceneManager.LoadScene("Failed_Level"); // Transition to the Main_Menu scene
    }
}

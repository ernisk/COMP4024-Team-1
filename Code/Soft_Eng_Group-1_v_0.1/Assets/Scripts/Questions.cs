using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Questions : MonoBehaviour
{
    // Assign the question image and answer in the inspector
    public Image question_image; 
    public string answer_number;

    // TEMP: StartQuestion method activated when Menu Button is clicked
    // REPLACE WITH PLAYER TRIGGER BY OBSTACLE OBJECT
    public void StartQuestion()
    {
        ShowQuestion();
    }

    void ShowQuestion()
    {
        if (question_image != null)
        {
            question_image.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Question image not assigned in inspector!");
        }
    }

    // Update if key pressed matches answer number
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && answer_number == "1")
        {
            CorrectAnswer();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && answer_number == "2")
        {
            CorrectAnswer();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && answer_number == "3")
        {
            CorrectAnswer();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && answer_number == "4")
        {
            CorrectAnswer();
        }
    }

    // Correct Answer Method, hiding question image and destroy obstacle object
    void CorrectAnswer()
    {
        if (question_image != null)
        {
            question_image.gameObject.SetActive(false);

            // ADD OBSTACLE OBJECT DESTROYED
        }
    }
}

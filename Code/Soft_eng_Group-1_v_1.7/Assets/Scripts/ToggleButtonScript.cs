using UnityEngine;
using UnityEngine.UI;

public class ToggleImageColorAndAudio : MonoBehaviour
{
    // Reference to the Toggle component.
    public Toggle toggleComponent;
    
    // Reference to the Image component (your PNG) that you want to change.
    public Image toggleImage;
    
    // Colors for the toggle states.
    public Color onColor = Color.white; // Normal color when toggle is on
    public Color offColor = Color.gray;   // Gray color when toggle is off

    void Start()
    {
        // If no Toggle reference is provided, try to get it from the current GameObject.
        if (toggleComponent == null)
            toggleComponent = GetComponent<Toggle>();

        // Set the toggle to be on by default.
        toggleComponent.isOn = true;

        // Subscribe to the toggle's onValueChanged event.
        toggleComponent.onValueChanged.AddListener(UpdateState);

        // Initialize the image color and audio sources based on the current state.
        UpdateState(toggleComponent.isOn);
        toggleComponent.interactable = true; 
    }

    // Update the image color and audio sources based on the toggle state.
    void UpdateState(bool isOn)
    {
        // Update image color.
        if (toggleImage != null)
        {
            toggleImage.color = isOn ? onColor : offColor;
        }

        // Enable or disable all AudioSources in the scene.
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in audioSources)
        {
            audio.enabled = isOn;
        }
    }
}

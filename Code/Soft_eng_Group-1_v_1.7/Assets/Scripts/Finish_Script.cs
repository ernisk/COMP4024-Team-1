using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class Finish : MonoBehaviour
{
    public RawImage videoDisplay; // Assign a scene RawImage in Inspector to hold the video
    public VideoClip videoClip; // Assign an MP4 video in Inspector of which video to present
    public string nextSceneName; // Change to next scene

    private VideoPlayer videoPlayer;
    private AudioSource audioSource;

    void Start()
    {
        // Create components
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        audioSource = gameObject.AddComponent<AudioSource>();

        // Setup VideoPlayer
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoClip;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);

        // Assign video to UI
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        videoPlayer.targetTexture = renderTexture;
        videoDisplay.texture = renderTexture;

        // Ensure the RawImage fits the screen
        videoDisplay.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

        // Subscribe to event when video finishes
        videoPlayer.loopPointReached += OnVideoEnd;

        // Play video
        videoPlayer.Play();
        audioSource.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName); // Load the next scene
    }
}

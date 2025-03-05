using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider2D))]
public class RealTimeSineGlissando : MonoBehaviour
{
    [Header("Sine Wave Settings")]
    [Tooltip("Duration of the sine wave (in seconds)")]
    public float duration = 2f; // Total duration

    [Tooltip("Starting frequency in Hz")]
    public float startFrequency = 440f; // Start frequency

    [Tooltip("Ending frequency in Hz")]
    public float endFrequency = 880f;   // End frequency

    [Header("Audio Settings")]
    [Tooltip("Volume for the sine wave (0 to 1)")]
    public float volume = 1f; // AudioSource volume

    // Internal state for real-time sine generation
    public bool generating = false;
    private float currentTime = 0f;
    private float phase = 0f;
    private float sampleRate;

    // Reference to the AudioSource (used only to drive the audio callback)
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Ensure a dummy silent clip is assigned so OnAudioFilterRead is called.
        if (audioSource.clip == null)
        {
            audioSource.clip = AudioClip.Create("Silent", 1, 1, AudioSettings.outputSampleRate, false);
        }
        audioSource.loop = true;
        audioSource.Play();

        sampleRate = AudioSettings.outputSampleRate;
    }

    void Start()
    {
        // Set the volume in Start so that any externally assigned volume is applied.
        audioSource.volume = volume;
    }

    /// <summary>
    /// Call this method to start generating the sine wave.
    /// </summary>
    public void TriggerSine()
    {
        generating = true;
        currentTime = 0f;
        phase = 0f;
        // Start a coroutine that destroys this GameObject after the specified duration.
        StartCoroutine(SelfDestruct());
    }

    // Coroutine to destroy the sine generator object after the duration is over.
    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    // When a collider with tag "Player" enters, create a new sine generator and destroy this object.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !generating)
        {
            // Create a new GameObject to continue generating the sine wave.
            GameObject sineGenerator = new GameObject("SineGenerator");
            // Add a BoxCollider2D to satisfy the [RequireComponent(typeof(Collider2D))] attribute.
            BoxCollider2D boxCollider = sineGenerator.AddComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;

            // Add the RealTimeSineGlissando component to the new GameObject.
            RealTimeSineGlissando newSine = sineGenerator.AddComponent<RealTimeSineGlissando>();
            // Copy parameter values.
            newSine.duration = this.duration;
            newSine.startFrequency = this.startFrequency;
            newSine.endFrequency = this.endFrequency;
            newSine.volume = this.volume;
            // The new instance will set up its own AudioSource in Awake and then apply volume in Start.
            // Start generating the sine wave on the new instance.
            newSine.TriggerSine();

            // Destroy this object immediately after starting the sound.
            Destroy(gameObject);
        }
    }

    // OnAudioFilterRead is called on the audio thread.
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!generating)
            return;

        for (int i = 0; i < data.Length; i += channels)
        {
            // Calculate normalized time (0 to 1).
            float tNorm = currentTime / duration;
            if (tNorm > 1f)
            {
                // Generation time is over; stop generating and zero out remaining samples.
                generating = false;
                for (int j = i; j < data.Length; j++)
                    data[j] = 0f;
                return;
            }

            // Linearly interpolate the frequency from startFrequency to endFrequency.
            float currentFreq = Mathf.Lerp(startFrequency, endFrequency, tNorm);

            // Compute amplitude envelope (piecewise linear):
            // 0% of duration: 0, 25%: 0.25, 50%: 1, 75%: 0.25, 100%: 0.
            float amplitude = 0f;
            if (tNorm <= 0.25f)
                amplitude = Mathf.Lerp(0f, 0.25f, tNorm / 0.25f);
            else if (tNorm <= 0.5f)
                amplitude = Mathf.Lerp(0.25f, 1f, (tNorm - 0.25f) / 0.25f);
            else if (tNorm <= 0.75f)
                amplitude = Mathf.Lerp(1f, 0.25f, (tNorm - 0.5f) / 0.25f);
            else
                amplitude = Mathf.Lerp(0.25f, 0f, (tNorm - 0.75f) / 0.25f);

            // Generate the sine sample.
            float sample = amplitude * Mathf.Sin(phase);

            // Write the sample to all channels.
            for (int ch = 0; ch < channels; ch++)
            {
                data[i + ch] = sample;
            }

            // Increment phase.
            phase += 2f * Mathf.PI * currentFreq / sampleRate;
            if (phase > 2f * Mathf.PI)
                phase -= 2f * Mathf.PI;

            // Increment current time by one sample duration.
            currentTime += 1f / sampleRate;
        }
    }
}

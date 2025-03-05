using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SineSoundGenerator : MonoBehaviour
{
    [Header("Sine Wave Parameters")]
    [Tooltip("Duration of the sound in seconds")]
    public float duration = 1.0f; // x seconds

    [Tooltip("Base frequency of the sine wave in Hz")]
    public float frequency = 440f; // y frequency

    [Tooltip("Amplitude of the sine wave (0 to 1)")]
    public float amplitude = 0.5f; // z amplitude

    [Header("Formant Frequencies")]
    [Tooltip("First formant frequency in Hz")]
    public float formant1 = 700f;

    [Tooltip("Second formant frequency in Hz")]
    public float formant2 = 1200f;

    [Tooltip("Third formant frequency in Hz")]
    public float formant3 = 2600f;

    [Header("Formant Amplitude Multipliers")]
    [Tooltip("Amplitude multiplier for the first formant")]
    public float formant1Amp = 0.5f;

    [Tooltip("Amplitude multiplier for the second formant")]
    public float formant2Amp = 0.3f;

    [Tooltip("Amplitude multiplier for the third formant")]
    public float formant3Amp = 0.2f;

    private AudioSource audioSource;
    private const int sampleRate = 44100;

    private void Awake()
    {
        // Get or add an AudioSource to play the sound.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Call this method (e.g., from a button click) to generate and play the sine sound.
    /// </summary>
    public void PlaySineSound()
    {
        int sampleCount = Mathf.CeilToInt(duration * sampleRate);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)sampleRate;
            // Generate the fundamental sine wave
            float sampleValue = amplitude * Mathf.Sin(2f * Mathf.PI * frequency * t);

            // Add the three formant sine waves (their amplitudes are scaled by their multipliers)
            sampleValue += amplitude * formant1Amp * Mathf.Sin(2f * Mathf.PI * formant1 * t);
            sampleValue += amplitude * formant2Amp * Mathf.Sin(2f * Mathf.PI * formant2 * t);
            sampleValue += amplitude * formant3Amp * Mathf.Sin(2f * Mathf.PI * formant3 * t);

            samples[i] = sampleValue;
        }

        // Create an AudioClip with the generated samples and play it using PlayOneShot.
        AudioClip clip = AudioClip.Create("SineSound", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        audioSource.PlayOneShot(clip);
    }
}

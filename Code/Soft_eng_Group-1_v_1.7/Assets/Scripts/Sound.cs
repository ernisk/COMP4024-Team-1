using System.Collections;
using UnityEngine;

public class VibraphoneSynth : MonoBehaviour
{
    private float[] originalPitchTable = {
        130.81f, 146.83f, 164.81f, 196.00f, 220.00f, 246.94f, 261.63f,
        293.66f, 329.63f, 392.00f, 440.00f, 493.88f, 523.25f, 587.33f, 659.25f,
        783.99f, 880.00f, 987.77f, 1046.50f
    }; // C3-C6 Pythagorean Tuning in C Major

    private float[] pitchTable;
    private int currentIndex;
    private int secondVoiceIndex;
    public float frequency;
    public float secondVoiceFrequency;
    private float sampleRate;
    private float time;
    private float secondVoiceTime;
    private bool playSound = false;
    private bool playSecondVoice = false;
    private float pitchMultiplier; // Pitch multiplier for each play

    // Base duration for notes and legato gap.
    // If both a "player" and "finish" are present, the baseDuration will adjust
    // as the player approaches the finish. Otherwise, it is randomized between 0.8 and 1.
    public float baseDuration = 1.0f;
    public float legatoGap = 0.05f;

    // Variables for adjusting baseDuration based on player's proximity to finish.
    private float initialBaseDuration = 1f;
    private float initialPlayerFinishDistance = 0f;
    private bool adjustBaseDuration = false;

    // ADSR parameters tailored for a vibraphone sound
    public float attackTime = 0.02f;
    public float attackCurve = 1.5f;
    public float decayTime = 0.15f;
    public float decayCurve = 1.8f;
    public float sustainLevel = 0.6f;
    public float releaseTime = 0.5f;
    public float releaseCurve = 2f;

    // Synth parameters tailored for a vibraphone sound
    public float vibratoFrequency = 6.0f;
    public float vibratoDepth = 0.01f;
    public float filterCutoff = 3000f;
    public float filterResonance = 1.2f;
    public float detuneAmount = 0.005f;

    // Oscillator parameters
    public enum Waveform { Sine, Triangle, Square, Sawtooth }
    public Waveform osc1Waveform = Waveform.Sine;
    public Waveform osc2Waveform = Waveform.Triangle;
    public Waveform osc3Waveform = Waveform.Sine;
    public float osc2FrequencyRatio = 1.5f;
    public float osc3FrequencyRatio = 2.0f;

    // LFO parameters
    public float lfoFrequency = 0.4f;
    public float lfoDepth = 0.05f;

    // FM Synthesis parameters
    public float fmIndex = 0.5f;

    // EQ settings to match vibraphone timbre
    public float lowShelfGain = 1.2f;
    public float midPeakGain = 0.8f;
    public float highShelfGain = 1.5f;

    private int trend = 0;
    private int trendCount = 0;
    private int transposition;
    private float noteAmplitude;
    private float currentDetune;

    // Fade-out management
    private float previousNoteAmplitude = 0.0f;
    private float fadeOutTime = 0.0f;
    private float fadeOutDuration = 0.5f;
    private float prob;

    void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;

        // Check for player and finish objects.
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        GameObject finishObj = GameObject.FindGameObjectWithTag("Finish");

        // If both exist, enable adjustment of baseDuration; otherwise, just randomize between 0.8 and 1.
        if (playerObj != null && finishObj != null)
        {
            adjustBaseDuration = true;
            initialBaseDuration = Random.Range(0.8f, 1f);
            baseDuration = initialBaseDuration;
            initialPlayerFinishDistance = Vector3.Distance(playerObj.transform.position, finishObj.transform.position);
        }
        else
        {
            adjustBaseDuration = false;
            baseDuration = Random.Range(0.8f, 1f);
            initialBaseDuration = baseDuration;
        }

        pitchMultiplier = Random.Range(0.75f, 1.5f);
        RandomizeOscillatorParameters();
        RandomizeADSRParameters();
        SetRandomValues();
        StartCoroutine(PlayNotesSequence());
    }

    // Update adjusts the baseDuration based on the player's proximity to the finish,
    // if both objects are present.
    void Update()
    {
        if (adjustBaseDuration)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            GameObject finishObj = GameObject.FindGameObjectWithTag("Finish");
            if (playerObj != null && finishObj != null && initialPlayerFinishDistance > 0)
            {
                float currentDistance = Vector3.Distance(playerObj.transform.position, finishObj.transform.position);
                float fraction = Mathf.Clamp01(currentDistance / initialPlayerFinishDistance);
                // When far away (fraction ~ 1), baseDuration is near the initial value.
                // When close (fraction -> 0), baseDuration approaches 0.35.
                baseDuration = Mathf.Lerp(0.35f, initialBaseDuration, fraction);
            }
        }
    }

    void RandomizeOscillatorParameters()
    {
        float sineChance = 0.35f;
        float triangleChance = 0.35f;
        float squareChance = 0.15f;
        float sawtoothChance = 0.15f;

        float[] waveformChances = new float[] {
            sineChance,
            sineChance + triangleChance,
            sineChance + triangleChance + squareChance,
            1.0f
        };

        osc1Waveform = GetRandomWaveform(waveformChances);
        osc2Waveform = GetRandomWaveform(waveformChances);
        osc3Waveform = GetRandomWaveform(waveformChances);

        osc2FrequencyRatio = new float[] {
            Random.Range(0.498f, 0.502f),
            Random.Range(0.99f, 1.02f),
            Random.Range(1.49f, 1.51f),
            Random.Range(1.96f, 2.05f)
        }[Random.Range(0, 4)];

        osc3FrequencyRatio = new float[] {
            Random.Range(0.499f, 0.501f),
            Random.Range(0.996f, 1.02f),
            Random.Range(1.498f, 1.52f),
            Random.Range(1.988f, 2.03f)
        }[Random.Range(0, 4)];

        fmIndex = Random.Range(0.4f, 0.6f);
    }

    Waveform GetRandomWaveform(float[] chances)
    {
        float randomValue = Random.value;
        if (randomValue <= chances[0])
            return Waveform.Sine;
        else if (randomValue <= chances[1])
            return Waveform.Triangle;
        else if (randomValue <= chances[2])
            return Waveform.Square;
        else
            return Waveform.Sawtooth;
    }

    void RandomizeADSRParameters()
    {
        attackTime = Random.Range(0.005f, 0.08f);
        decayTime = Random.Range(0.1f, 0.7f);
        sustainLevel = Random.Range(0.05f, 0.3f);
        releaseTime = Random.Range(0.4f, 0.6f);
    }

    void SetRandomValues()
    {
        transposition = Random.Range(-7, 8);
        pitchTable = new float[originalPitchTable.Length];
        for (int i = 0; i < originalPitchTable.Length; i++)
        {
            pitchTable[i] = originalPitchTable[i] * Mathf.Pow(2f, transposition / 12f) * pitchMultiplier;
        }
        currentIndex = Random.Range(0, pitchTable.Length);
        secondVoiceIndex = Mathf.Min(currentIndex + 4, pitchTable.Length - 1);

        noteAmplitude = Random.Range(0.9f, 1.1f);
        currentDetune = Random.Range(-detuneAmount, detuneAmount);

        // Reinitialize baseDuration based on adjustment mode.
        if (adjustBaseDuration)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            GameObject finishObj = GameObject.FindGameObjectWithTag("Finish");
            if (playerObj != null && finishObj != null)
            {
                initialPlayerFinishDistance = Vector3.Distance(playerObj.transform.position, finishObj.transform.position);
            }
            initialBaseDuration = Random.Range(0.8f, 1f);
            baseDuration = initialBaseDuration;
        }
        else
        {
            baseDuration = Random.Range(0.8f, 1f);
            initialBaseDuration = baseDuration;
        }
    }

    IEnumerator PlayNotesSequence()
    {
        playSound = true;
        while (playSound)
        {
            frequency = pitchTable[currentIndex];
            secondVoiceFrequency = pitchTable[secondVoiceIndex];
            time = 0f;
            fadeOutTime = 0f;
            if (playSecondVoice)
            {
                secondVoiceTime = 0f;
            }
            float currentDuration = GetRenaissanceNoteDuration();
            currentIndex = GetNextNoteIndex(currentIndex);
            secondVoiceIndex = Mathf.Min(currentIndex + 4, pitchTable.Length - 1);
            yield return new WaitForSeconds(currentDuration - legatoGap);
            playSecondVoice = true;
            noteAmplitude = Random.Range(0.9f, 1.1f);
            currentDetune = Random.Range(-detuneAmount, detuneAmount);
        }
    }

    void StartFadeOut()
    {
        previousNoteAmplitude = noteAmplitude;
        fadeOutTime = 0f;
        StartCoroutine(FadeOutPreviousNote());
    }

    IEnumerator FadeOutPreviousNote()
    {
        while (fadeOutTime < fadeOutDuration)
        {
            fadeOutTime += Time.deltaTime;
            yield return null;
        }
        playSound = false;
    }

    float GetRenaissanceNoteDuration()
    {
        float[] noteValues = { 1f, 0.5f };
        prob = Random.Range(0.2f, 0.6f);
        float[] probabilities = { prob, 1f - prob };
        float cumulativeProbability = 0f;
        float randomValue = Random.value;
        for (int i = 0; i < noteValues.Length; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                return baseDuration * noteValues[i];
            }
        }
        return baseDuration;
    }

    int GetNextNoteIndex(int currentIndex)
    {
        int[] possibleMovements = { 0, 2, 4, 7, -5, -2 };
        if (currentIndex >= pitchTable.Length - 4)
        {
            trend = -1;
            trendCount = 0;
        }
        if (trend == -1 && trendCount < 3)
        {
            possibleMovements = new int[] { -5, -2 };
            trendCount++;
        }
        else
        {
            trend = 0;
            trendCount = 0;
        }
        int movement = possibleMovements[Random.Range(0, possibleMovements.Length)];
        int newIndex = currentIndex + movement;
        if (newIndex < 0)
        {
            newIndex = 0;
            trend = 1;
            trendCount = 1;
        }
        else if (newIndex >= pitchTable.Length)
        {
            newIndex = pitchTable.Length - 1;
            trend = -1;
            trendCount = 1;
        }
        return newIndex;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!playSound) return;
        int dataLength = data.Length / channels;
        for (int i = 0; i < dataLength; i++)
        {
            float vibrato = Mathf.Sin(2 * Mathf.PI * vibratoFrequency * time) * vibratoDepth;
            float lfo = Mathf.Sin(2 * Mathf.PI * lfoFrequency * time) * lfoDepth;
            float t = time / baseDuration;
            float amplitude = GetAdvancedEnvelopeValue(t) * noteAmplitude;

            float osc1Freq = frequency + currentDetune + vibrato + lfo;
            float osc2Freq = osc1Freq * osc2FrequencyRatio;
            float osc3Freq = osc1Freq * osc3FrequencyRatio;

            float fmOsc1 = GenerateWaveform(osc1Waveform, time, osc1Freq);
            float fmOsc2 = GenerateWaveform(osc2Waveform, time, osc2Freq);
            float fmOsc3 = GenerateWaveform(osc3Waveform, time, osc3Freq);
            float firstVoiceSample = fmOsc1 * fmOsc2 * fmOsc3 * amplitude;

            float secondVoiceSample = 0f;
            if (playSecondVoice)
            {
                float secondVoiceAmplitude = GetAdvancedEnvelopeValue(secondVoiceTime / baseDuration) * noteAmplitude;
                float osc1FreqSV = secondVoiceFrequency + currentDetune + vibrato + lfo;
                float osc2FreqSV = osc1FreqSV * osc2FrequencyRatio;
                float osc3FreqSV = osc1FreqSV * osc3FrequencyRatio;
                float fmOsc1SV = GenerateWaveform(osc1Waveform, secondVoiceTime, osc1FreqSV);
                float fmOsc2SV = GenerateWaveform(osc2Waveform, secondVoiceTime, osc2FreqSV);
                float fmOsc3SV = GenerateWaveform(osc3Waveform, secondVoiceTime, osc3FreqSV);
                secondVoiceSample = fmOsc1SV * fmOsc2SV * fmOsc3SV * secondVoiceAmplitude;
                secondVoiceTime += 1f / sampleRate;
            }

            firstVoiceSample = ApplyEQ(firstVoiceSample);
            firstVoiceSample = LowPassFilter(firstVoiceSample, filterCutoff);
            secondVoiceSample = ApplyEQ(secondVoiceSample);
            secondVoiceSample = LowPassFilter(secondVoiceSample, filterCutoff);

            for (int j = 0; j < channels; j++)
            {
                if (j == 0)
                    data[i * channels + j] = secondVoiceSample;
                else if (j == 1)
                    data[i * channels + j] = firstVoiceSample;
            }
            time += 1f / sampleRate;
            if (time > baseDuration)
            {
                break;
            }
        }
    }

    float GenerateWaveform(Waveform waveform, float t, float freq)
    {
        switch (waveform)
        {
            case Waveform.Sine:
                return Mathf.Sin(2 * Mathf.PI * freq * t);
            case Waveform.Square:
                return Mathf.Sign(Mathf.Sin(2 * Mathf.PI * freq * t));
            case Waveform.Triangle:
                return Mathf.PingPong(2 * freq * t, 1f) * 2f - 1f;
            case Waveform.Sawtooth:
                return 2f * (t * freq - Mathf.Floor(0.5f + t * freq));
            default:
                return Mathf.Sin(2 * Mathf.PI * freq * t);
        }
    }

    float LowPassFilter(float input, float freq)
    {
        float RC = 1.0f / (filterCutoff * 2 * Mathf.PI);
        float dt = 1.0f / sampleRate;
        float alpha = dt / (RC + dt);
        float previousSample = 0.0f;
        float filteredSample = alpha * input + (1 - alpha) * previousSample;
        previousSample = filteredSample;
        return filteredSample * filterResonance;
    }

    float ApplyEQ(float sample)
    {
        float lowShelf = sample * lowShelfGain;
        float midPeak = sample * midPeakGain;
        float highShelf = sample * highShelfGain;
        return lowShelf + midPeak + highShelf;
    }

    float GetAdvancedEnvelopeValue(float t)
    {
        float amplitude;
        if (t < attackTime)
            amplitude = Mathf.Pow(t / attackTime, attackCurve);
        else if (t < attackTime + decayTime)
        {
            float decayT = (t - attackTime) / decayTime;
            amplitude = Mathf.Lerp(1f, sustainLevel, Mathf.Pow(decayT, decayCurve));
        }
        else if (t < 1f - releaseTime)
            amplitude = sustainLevel;
        else
        {
            float releaseT = (t - (1f - releaseTime)) / releaseTime;
            amplitude = Mathf.Lerp(sustainLevel, 0f, Mathf.Pow(releaseT, releaseCurve));
        }
        return amplitude;
    }

    public float GetCurrentAmplitude()
    {
        float t = time / baseDuration;
        return GetAdvancedEnvelopeValue(t) * noteAmplitude;
    }
}
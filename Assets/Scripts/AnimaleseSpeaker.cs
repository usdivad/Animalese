/**
 * This class handles Animalese audio playback
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimaleseSpeaker : MonoBehaviour
{
    /**
     * The parent to all the event paths.
     *
     * Assumes all events have structure "{parentPath}{letterName}",
     * e.g. "event:/AnimaleseBase/A"
     */
    [SerializeField]
    string m_EventParentPath = "event:/";

    /**
     *  Time (in seconds) to wait before "speaking" the next letter.
     *  The lower this is, the faster the speech will be.
     *
     *  Pitch is not affected.
     */  
        
    [SerializeField]
    float m_PlaybackTimeBetweenLetters = 0.25f;

    /**
     * Amount to pitch-shift letter sounds, as a multiple of the original pitch.
     * The higher this is, the higher the pitch will be.
     *
     * For example, a value of 2 will double the pitch, 1 will retain the
     * original pitch, and 0.5 will halve the pitch.
     *
     * Speed is not affected.
     */
    [SerializeField]
    float m_PlaybackPitch = 2f;

    /**
     * Whether to loop playback once the specified text is done playing
     */
    [SerializeField]
    bool m_ShouldLoopPlayback = false;

    /**
     * Private member variables, mostly to keep track of current state
     */
    FMOD.Studio.EventInstance m_CurrentEventInstance = new FMOD.Studio.EventInstance();
    string m_TextToSpeak = "";
    string m_TextToDisplay = "";
    string m_CurrentTextSpoken = "";
    float m_CurrentLetterElapsedTime = 0f;
    int m_CurrentLetterIndex = 0;
    bool m_IsPlaying = false;
    bool m_IsCurrentLetterEventPathValid = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsPlaying)
        {
            UpdatePlayback();
        }
    }


    // ================================

    /**
     * This function is called once per frame during playback, and handles the
     * logic for what audio to play and when to play it
     */
    void UpdatePlayback()
    {
        if (m_CurrentLetterElapsedTime >= m_PlaybackTimeBetweenLetters)
        {
            // Play audio for the curent letter
            char currentLetter = m_TextToSpeak[m_CurrentLetterIndex];
            PlayAudioForLetter(currentLetter);

            // Display letter with original upper/lower casing
            char currentLetterToDisplay = m_TextToDisplay[m_CurrentLetterIndex];
            m_CurrentTextSpoken += currentLetterToDisplay;
            //Debug.Log(m_CurrentTextSpoken);

            // Increment letter index and reset letter elapsed time
            m_CurrentLetterIndex++;
            m_CurrentLetterElapsedTime = 0f;

            // Handle case for when we've reached the end of the text
            if (m_CurrentLetterIndex >= m_TextToSpeak.Length)
            {
                if (m_ShouldLoopPlayback)
                {
                    // Reset letter index to loop back from beginning
                    m_CurrentLetterIndex = 0;
                    m_CurrentTextSpoken = "";
                }
                else
                {
                    // Stop playing the text -- we're done
                    StopPlayback();
                }
            }
        }
        else
        {
            // Increment letter elapsed time
            m_CurrentLetterElapsedTime += Time.deltaTime;
        }
    }

    /**
     * This function is called whenever a new letter's audio needs to be
     * played, and triggers the correct event given the letter specified
     */
    void PlayAudioForLetter(char letter)
    {        
        // Stop current event
        if (m_CurrentEventInstance.isValid())
        {
            m_CurrentEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        // Construct event path
        string eventPath = m_EventParentPath + letter;

        // Make sure event path is valid
        FMOD.Studio.EventDescription eventDescription;
        FMOD.RESULT result = FMODUnity.RuntimeManager.StudioSystem.getEvent(eventPath, out eventDescription);
        bool isEventPathValid = (result == FMOD.RESULT.OK);
        m_IsCurrentLetterEventPathValid = isEventPathValid;

        if (isEventPathValid)
        {
            // Create new event
            m_CurrentEventInstance = FMODUnity.RuntimeManager.CreateInstance(eventPath);

            // Set attributes
            m_CurrentEventInstance.setPitch(m_PlaybackPitch);

            // Play new event
            m_CurrentEventInstance.start();
            //Debug.LogFormat("PlayAudioForLetter: {0}", eventPath);
        }
    }

    // ================================

    public void StartPlayback()
    {
        // Start playing if there's text to speak
        if (m_TextToSpeak.Length > 0)
        {
            m_IsPlaying = true;
        }
    }

    public void StopPlayback()
    {
        // Stop playback and reset values
        m_IsPlaying = false;
        m_CurrentLetterIndex = 0;
        m_CurrentTextSpoken = "";
    }

    public bool IsPlaying()
    {
        return m_IsPlaying;
    }

    public bool IsCurrentLetterEventPathValid()
    {
        return m_IsCurrentLetterEventPathValid;
    }

    public float GetCurrentLetterElapsedPercentage()
    {
        return m_CurrentLetterElapsedTime / m_PlaybackTimeBetweenLetters;
    }

    public char GetCurrentLetter()
    {
        return (m_TextToSpeak.Length > m_CurrentLetterIndex) ? m_TextToSpeak[m_CurrentLetterIndex] : ' ';
    }

    public string GetCurrentTextSpoken()
    {
        return m_CurrentTextSpoken;
    }

    public string GetTextToDisplay()
	{
		return m_TextToDisplay;
	}

    public void SetTextToSpeak(string textInput)
    {
        // Set all letters to uppercase in order to match event names
        m_TextToSpeak = textInput.ToUpper();
        m_TextToDisplay = textInput;
        //Debug.LogFormat("SetEventLettersFromTextInput: {0}", m_TextToSpeak);

        // Stop playback to prevent indexing errors
        StopPlayback();
    }

    public void SetPlaybackPitch(float pitch)
    {
        m_PlaybackPitch = pitch;
    }

    public void SetPlaybackTimeBetweenLetters(float seconds)
    {
        m_PlaybackTimeBetweenLetters = seconds;
    }

    public void SetPlaybackTimeBetweenLettersFromBPM(float bpm)
    {
        float seconds = 60f / bpm;
        SetPlaybackTimeBetweenLetters(seconds);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalesePlayer : MonoBehaviour
{
    // TODO: Add documentation
    [SerializeField]
    string m_EventParentPath = "event:/";
    [SerializeField]
    float m_PlaybackTimeBetweenLetters = 0.25f;
    [SerializeField]
    float m_PlaybackPitch = 2f;
    [SerializeField]
    bool m_ShouldLoopPlayback = false;

    FMOD.Studio.EventInstance m_CurrentEventInstance = new FMOD.Studio.EventInstance();
    string m_TextToSpeak = "";
    float m_CurrentLetterElapsedTime = 0f;
    int m_CurrentLetterIndex = 0;
    bool m_IsPlaying = false;

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

    void UpdatePlayback()
    {
        if (m_CurrentLetterElapsedTime >= m_PlaybackTimeBetweenLetters)
        {
            // Play audio for the curent letter
            char currentLetter = m_TextToSpeak[m_CurrentLetterIndex];
            PlayAudioForLetter(currentLetter);

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

        if (isEventPathValid)
        {
            // Create new event
            m_CurrentEventInstance = FMODUnity.RuntimeManager.CreateInstance(eventPath);

            // Set attributes
            m_CurrentEventInstance.setPitch(m_PlaybackPitch);

            // Play new event
            m_CurrentEventInstance.start();
            Debug.LogFormat("PlayAudioForLetter: {0}", eventPath);
        }
    }

    // ================================

    public void StartPlayback()
    {
        m_IsPlaying = true;
    }

    public void StopPlayback()
    {
        m_IsPlaying = false;
        m_CurrentLetterIndex = 0;
    }

    public void SetTextToSpeak(string textInput)
    {
        // Set all letters to uppercase in order to match event names
        m_TextToSpeak = textInput.ToUpper();
        Debug.LogFormat("SetEventLettersFromTextInput: {0}", m_TextToSpeak);

        // Stop playback to prevent indexing errors
        StopPlayback();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalesePlayer : MonoBehaviour
{
    // TODO: Add documentation
    [SerializeField]
    string m_EventParentPath = "event:/";
    [SerializeField]
    string[] m_EventLetterNames;
    [SerializeField]
    float m_PlaybackSpeed = 1f;
    [SerializeField]
    float m_PlaybackPitch = 1f;

    FMOD.Studio.EventInstance m_CurrentEventInstance = new FMOD.Studio.EventInstance();
    float m_CurrentLetterElapsedTime = 0f;
    int m_CurrentLetterIndex = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_CurrentLetterElapsedTime >= m_PlaybackSpeed)
        {
            string currentLetter = m_EventLetterNames[m_CurrentLetterIndex];
            PlayAudioForLetter(currentLetter);

            m_CurrentLetterIndex = (m_CurrentLetterIndex + 1) % m_EventLetterNames.Length;
            m_CurrentLetterElapsedTime = 0f;
        }

        m_CurrentLetterElapsedTime += Time.deltaTime;
    }


    // ================================

    void PlayAudioForLetter(string letter)
    {
        // Construct event path
        string eventPath = m_EventParentPath + letter;

        // Stop current event
        m_CurrentEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        // Create new event and set attributes
        m_CurrentEventInstance = FMODUnity.RuntimeManager.CreateInstance(eventPath);
        m_CurrentEventInstance.setPitch(m_PlaybackPitch);

        // Play new event
        m_CurrentEventInstance.start();

        Debug.Log(eventPath);
    }

}

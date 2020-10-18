using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimaleseSpeaker))]
public class AnimaleseSpeakerAnimationHandler : MonoBehaviour
{
    [SerializeField]
    Animator m_Animator;
    [SerializeField]
    GameObject m_TalkingFace;
    [SerializeField]
    GameObject m_IdleFace;
    [SerializeField]
    bool m_ShouldAnimateMouth = false;
    [SerializeField]
    float m_LetterMouthOpenPercentage = 0.5f;

    static string m_VowelLetters = "AEIOUY";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AnimaleseSpeaker speaker = GetComponent<AnimaleseSpeaker>();
        bool isTalking = speaker.IsPlaying();
        bool isCurrentLetterAVowel = m_VowelLetters.Contains(speaker.GetCurrentLetter().ToString());
        bool isMouthOpen = isCurrentLetterAVowel
                           // && speaker.IsCurrentLetterEventPathValid()
                           // && speaker.GetCurrentLetterElapsedPercentage() <= m_LetterMouthOpenPercentage
                           && isTalking
                           && m_ShouldAnimateMouth;

        m_Animator.SetBool("IsTalking", isTalking);
        m_TalkingFace.SetActive(isMouthOpen);
        m_IdleFace.SetActive(!isMouthOpen);
    }

    // ================================

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class AnimaleseTextDisplayHandler : MonoBehaviour
{
    [SerializeField]
    AnimaleseSpeaker m_AnimaleseSpeaker;

    [SerializeField]
    TMP_InputField m_InputField;

    [SerializeField]
    UnityEngine.UI.Image m_DisplayImage;

    [SerializeField]
    TMP_Text m_DisplayText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isSpeaking = m_AnimaleseSpeaker.IsPlaying();

        m_InputField.enabled = !isSpeaking;

        m_DisplayImage.enabled = isSpeaking;
        m_DisplayText.enabled = isSpeaking;

        m_DisplayText.text = m_AnimaleseSpeaker.GetCurrentTextSpoken();
    }
}

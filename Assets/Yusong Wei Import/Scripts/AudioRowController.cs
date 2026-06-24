using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioRowController : MonoBehaviour
{
    [Header("set")]
    public AudioSource targetAudioSource; // audio
    public AudioMixer mainMixer;          // audio place

    [Header("UI")]
    public Toggle playToggle;
    public Slider lowSlider;
    public Slider midSlider;
    public Slider highSlider;

    [Header("audio name")]
    public string lowParamName = "Audio1_Low";
    public string midParamName = "Audio1_Mid";
    public string highParamName = "Audio1_High";

    private void Start()
    {
       
        playToggle.onValueChanged.AddListener(OnToggleChanged);
        lowSlider.onValueChanged.AddListener(SetLowEQ);
        midSlider.onValueChanged.AddListener(SetMidEQ);
        highSlider.onValueChanged.AddListener(SetHighEQ);

  
        OnToggleChanged(playToggle.isOn);
    }


    private void OnToggleChanged(bool isOn)
    {
        if (targetAudioSource == null) return;

        if (isOn)
        {
            if (!targetAudioSource.isPlaying) targetAudioSource.Play();
        }
        else
        {
            targetAudioSource.Pause(); 
        }
    }

    // Low
    private void SetLowEQ(float value)
    {
        if (mainMixer != null) mainMixer.SetFloat(lowParamName, value);
    }

    // Mid
    private void SetMidEQ(float value)
    {
        if (mainMixer != null) mainMixer.SetFloat(midParamName, value);
    }

    // High
    private void SetHighEQ(float value)
    {
        if (mainMixer != null) mainMixer.SetFloat(highParamName, value);
    }
}
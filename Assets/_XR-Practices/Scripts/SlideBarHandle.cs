using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlideBarHandle : MonoBehaviour
{
    public SliderType sliderType;
    public Slider slide;
    public TextMeshProUGUI fontText;
    public AudioSource audioSource;

    private void Awake()
    {
        slide.onValueChanged.AddListener(OnSliderValueChanged);
    }
    private void Start()
    {
        StartCoroutine(SyncValue());
    }

    private IEnumerator SyncValue()
    {
        while (true)
        {
            switch (sliderType)
            {
                case SliderType.Font:
                    slide.value = fontText.fontSize * 2;
                    break;
                case SliderType.Audio:
                    slide.value = audioSource.volume;
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnSliderValueChanged(float value)
    {
        switch (sliderType)
        {
            case SliderType.Font:
                SetFontValue(value);
                break;
            case SliderType.Audio:
                SetAudioValue(value);
                break;
            default:
                break;
        }
    }
    private void SetAudioValue(float value)
    {
        if (!audioSource) return;
        audioSource.volume = value;
    }
    private void SetFontValue(float Value)
    {
        if (!fontText) return;
        fontText.fontSize = Value/2;
    }
}
public enum SliderType
{
    Font,
    Audio
}

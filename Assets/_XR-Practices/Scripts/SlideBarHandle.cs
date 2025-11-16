using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlideBarHandle : MonoBehaviour
{
    public SliderType sliderType;
    public Slider slide;
    //[Range(56, 138)]
    public Vector2 fontSizeRange;
    public Text[] fontText;
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
                    slide.value = FormatFontSize(fontText[0].fontSize);
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
        if (fontText == null || fontText.Length < 1) return;
        foreach (var text in fontText)
        {
            text.fontSize = ReformatFontSize(Value);
        }
    }

    /// <summary>
    /// 格式化字体大小
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private float FormatFontSize(int size)
    {
        float min = (int)fontSizeRange.x;
        float max = (int)fontSizeRange.y;
        float curr = size;
        return (curr - min) / (max - min);
    }
    /// <summary>
    /// 解析字体大小
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private int ReformatFontSize(float size)
    {
        float min = (int)fontSizeRange.x;
        float max = (int)fontSizeRange.y;
        float curr = size;
        return (int)(curr * (max - min) + min);
    }
}
public enum SliderType
{
    Font,
    Audio
}

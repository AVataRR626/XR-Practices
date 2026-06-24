using UnityEngine;
using UnityEngine.UI;

public class AudioPreset : MonoBehaviour
{
    [Header("=== (Sliders) ===")]
    [Tooltip("ADD Slider")]
    public Slider[] targetSliders;

    [Header("=== preset ===")]
    [Tooltip("keep number and value")]
    public float[] presetValues;

    // button OnClick event
    public void ApplyPreset()
    {
        // 1. safety check
        if (targetSliders.Length != presetValues.Length)
        {
            Debug.LogError("error, check Inspector");
            return;
        }

        // 2. traverse the slider and modify value
        for (int i = 0; i < targetSliders.Length; i++)
        {
            if (targetSliders[i] != null)
            {
                //  value change Slider: OnValueChanged event
                // AudioMixer self-synchronizing
                targetSliders[i].value = presetValues[i];
            }
        }
    }
}
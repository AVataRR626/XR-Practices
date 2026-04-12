using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GlobalEQController : MonoBehaviour
{
    [Header("=== Call AudioMixer ===")]
    public AudioMixer mainMixer;

    [Header("=== 6 UI slider ===")]
    public Slider volSlider;
    public Slider subSlider;
    public Slider lowMidSlider;
    public Slider midSlider;
    public Slider highMidSlider;
    public Slider airSlider;

    private void Start()
    {
        // Call slider and edit AudioMixer
        volSlider.onValueChanged.AddListener(val => mainMixer.SetFloat("Master_Vol", val));
        subSlider.onValueChanged.AddListener(val => mainMixer.SetFloat("Master_Sub", val));
        lowMidSlider.onValueChanged.AddListener(val => mainMixer.SetFloat("Master_LowMid", val));
        midSlider.onValueChanged.AddListener(val => mainMixer.SetFloat("Master_Mid", val));
        highMidSlider.onValueChanged.AddListener(val => mainMixer.SetFloat("Master_HighMid", val));
        airSlider.onValueChanged.AddListener(val => mainMixer.SetFloat("Master_Air", val));
    }

    // board= active ,(SetActive=true),then
    private void OnEnable()
    {
        if (mainMixer == null) return;
        
        float val;
        // Auto reset
        if (mainMixer.GetFloat("Master_Vol", out val)) volSlider.SetValueWithoutNotify(val);
        if (mainMixer.GetFloat("Master_Sub", out val)) subSlider.SetValueWithoutNotify(val);
        if (mainMixer.GetFloat("Master_LowMid", out val)) lowMidSlider.SetValueWithoutNotify(val);
        if (mainMixer.GetFloat("Master_Mid", out val)) midSlider.SetValueWithoutNotify(val);
        if (mainMixer.GetFloat("Master_HighMid", out val)) highMidSlider.SetValueWithoutNotify(val);
        if (mainMixer.GetFloat("Master_Air", out val)) airSlider.SetValueWithoutNotify(val);
    }
}
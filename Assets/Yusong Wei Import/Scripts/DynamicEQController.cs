using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class DynamicEQController : MonoBehaviour
{
    [Header("=== UI 控件绑定 | UI Contol Binding===")]
    public TextMeshProUGUI currentTrackText; // (Now Editing...)
    public Toggle playToggle;                // select board
    public Slider lowSlider, midSlider, highSlider; // controller Mid，High，Low

    [Header("=== 左侧频道切换按钮 | Left-side Switching Buttons===")]
    public Button[] trackButtons;            // 3 BUTTON

    [Header("=== 混音器引用 | Mixer Reference ===")]
    public AudioMixer mainMixer;

    // Audio mixer
    private AudioSource[] currentSources;
    private string[] currentNames;
    private string[] lowParams, midParams, highParams;

    private int activeIndex = 0; // choose which Audio (0, 1, 2)

    private void Start()
    {
        // controller
        lowSlider.onValueChanged.AddListener(val => { if (HasActive()) mainMixer.SetFloat(lowParams[activeIndex], val); });
        midSlider.onValueChanged.AddListener(val => { if (HasActive()) mainMixer.SetFloat(midParams[activeIndex], val); });
        highSlider.onValueChanged.AddListener(val => { if (HasActive()) mainMixer.SetFloat(highParams[activeIndex], val); });

        // select
        playToggle.onValueChanged.AddListener(isOn => 
        {
            if (HasActive())
            {
                if (isOn && !currentSources[activeIndex].isPlaying) currentSources[activeIndex].Play();
                else if (!isOn && currentSources[activeIndex].isPlaying) currentSources[activeIndex].Pause();
            }
        });

        // 
        if (trackButtons.Length >= 3)
        {
            trackButtons[0].onClick.AddListener(() => SwitchTrack(0));
            trackButtons[1].onClick.AddListener(() => SwitchTrack(1));
            trackButtons[2].onClick.AddListener(() => SwitchTrack(2));
        }
    }

    private bool HasActive() { return currentSources != null && currentSources.Length > activeIndex && currentSources[activeIndex] != null; }

    // receive data
    public void SetupPanelGroup(AudioSource[] sources, string[] names, string[] lows, string[] mids, string[] highs)
    {
        currentSources = sources;
        currentNames = names;
        lowParams = lows;
        midParams = mids;
        highParams = highs;

        // update text Editing Audio 1
        for (int i = 0; i < 3; i++)
        {
            if (i < trackButtons.Length && trackButtons[i] != null)
                trackButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = names[i];
        }

        SwitchTrack(0); // 
    }

    // auto reset
    public void SwitchTrack(int index)
    {
        activeIndex = index;
        if (currentTrackText != null) currentTrackText.text = "Now Editing: " + currentNames[index];

        if (!HasActive()) return;

        // on/off state
        playToggle.SetIsOnWithoutNotify(currentSources[activeIndex].isPlaying);

        // force update the position of the slider
        float val;
        if (mainMixer.GetFloat(lowParams[index], out val)) lowSlider.SetValueWithoutNotify(val);
        if (mainMixer.GetFloat(midParams[index], out val)) midSlider.SetValueWithoutNotify(val);
        if (mainMixer.GetFloat(highParams[index], out val)) highSlider.SetValueWithoutNotify(val);
    }
}
using UnityEngine;
using UnityEngine.Audio;
using static Unity.VisualScripting.Member;

public class AudioMixerChange : MonoBehaviour
{
    
    [Header("==Background Speech Settings==")]

    [Tooltip("The AudioMixerGroup you want to detect. Choose your background speech mixer.")]
    public AudioMixerGroup backgroundAudioMixer;

    [Tooltip("How spatialized the audio is before detection. Recommend 1 for 3D spatialization.")]
    public float backgroundSpatialBlend = 1;


    
    [Header("==Foreground Speech Settings==")]
  
    [Tooltip("The new AudioMixerGroup you want to assign. Choose your foreground speech mixer.")]
    public AudioMixerGroup foregroundAudioMixer;

    [Tooltip("How spatialized the audio source becomes. Recommend 0 for 2D spatialization.")]
    public float foregroundSpatialBlend = 0;


    private void OnTriggerEnter(Collider other)
    {
        // 1. Check if the entering object has an AudioSource
        AudioSource audioSource = other.GetComponent<AudioSource>();

        if (audioSource != null && audioSource.outputAudioMixerGroup != null)
        {
            // 2. Check if the output mixer group matches the specific name
            if (audioSource.outputAudioMixerGroup == backgroundAudioMixer)
            {
                Debug.Log($"AudioTriggerArea: Object {other.name} entered while using the {backgroundAudioMixer} mixer!");

                //3. Change to the new mixer group
                SwitchMixerGroupFore(audioSource);

                //4. New spatial float
                SetSpatialFloatFore(audioSource);

                
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // 1. Check if the entering object has an AudioSource
        AudioSource audioSource = other.GetComponent<AudioSource>();

        if (audioSource != null && audioSource.outputAudioMixerGroup != null)
        {
            // 2. Check if the output mixer group matches the specific name
            if (audioSource.outputAudioMixerGroup == foregroundAudioMixer)
            {
                Debug.Log($"AudioTriggerArea: Object {other.name} exited while using the {foregroundAudioMixer} mixer!");

                //3. Change to the new mixer group
                SwitchMixerGroupBack(audioSource);

                //4. New spatial float
                SetSpatialFloatBack(audioSource);
            }
        }
    }
    private void SwitchMixerGroupFore(AudioSource audioSource)
    { 
        audioSource.outputAudioMixerGroup = foregroundAudioMixer;
    }
    private void SwitchMixerGroupBack(AudioSource audioSource)
    {
        audioSource.outputAudioMixerGroup = backgroundAudioMixer;
    }
        
    private void SetSpatialFloatFore(AudioSource audioSource)
    {
        audioSource.spatialBlend = foregroundSpatialBlend;
    }

    private void SetSpatialFloatBack(AudioSource audioSource)
    { audioSource.spatialBlend = backgroundSpatialBlend;}
    

}



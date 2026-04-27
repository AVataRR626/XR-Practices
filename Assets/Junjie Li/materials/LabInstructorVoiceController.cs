using System.Collections;
using UnityEngine;

public class LabInstructorVoiceController : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource voiceAudioSource;

    [Header("Voice Clips")]
    [SerializeField] private AudioClip pickUpBeakerClip;
    [SerializeField] private AudioClip invalidPlacementClip;
    [SerializeField] private AudioClip reagentLoadedClip;
    [SerializeField] private AudioClip analyzingClip;
    [SerializeField] private AudioClip tooShortClip;
    [SerializeField] private AudioClip completeClip;
    [SerializeField] private AudioClip tooLongClip;

    [Header("Playback Settings")]
    [SerializeField] private bool stopCurrentVoiceBeforePlaying = true;
    [SerializeField] private bool playPickUpVoiceOnlyOnce = true;

    private bool hasPlayedPickUpVoice = false;
    private Coroutine delayedVoiceRoutine;

    public void PlayPickUpBeakerVoice()
    {
        if (playPickUpVoiceOnlyOnce && hasPlayedPickUpVoice)
        {
            return;
        }

        hasPlayedPickUpVoice = true;
        PlayVoice(pickUpBeakerClip);
    }

    public void PlayInvalidPlacementVoiceDelayed(float delay)
    {
        PlayVoiceDelayed(invalidPlacementClip, delay);
    }

    public void PlayReagentLoadedVoiceDelayed(float delay)
    {
        PlayVoiceDelayed(reagentLoadedClip, delay);
    }

    public void PlayAnalyzingVoiceDelayed(float delay)
    {
        PlayVoiceDelayed(analyzingClip, delay);
    }

    public void PlayTooShortVoiceDelayed(float delay)
    {
        PlayVoiceDelayed(tooShortClip, delay);
    }

    public void PlayCompleteVoiceDelayed(float delay)
    {
        PlayVoiceDelayed(completeClip, delay);
    }

    public void PlayTooLongVoiceDelayed(float delay)
    {
        PlayVoiceDelayed(tooLongClip, delay);
    }

    private void PlayVoiceDelayed(AudioClip clip, float delay)
    {
        if (voiceAudioSource == null || clip == null)
        {
            return;
        }

        if (delayedVoiceRoutine != null)
        {
            StopCoroutine(delayedVoiceRoutine);
            delayedVoiceRoutine = null;
        }

        delayedVoiceRoutine = StartCoroutine(DelayedVoiceRoutine(clip, delay));
    }

    private IEnumerator DelayedVoiceRoutine(AudioClip clip, float delay)
    {
        if (stopCurrentVoiceBeforePlaying && voiceAudioSource.isPlaying)
        {
            voiceAudioSource.Stop();
        }

        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        voiceAudioSource.clip = clip;
        voiceAudioSource.Play();

        delayedVoiceRoutine = null;
    }

    private void PlayVoice(AudioClip clip)
    {
        if (voiceAudioSource == null || clip == null)
        {
            return;
        }

        if (delayedVoiceRoutine != null)
        {
            StopCoroutine(delayedVoiceRoutine);
            delayedVoiceRoutine = null;
        }

        if (stopCurrentVoiceBeforePlaying && voiceAudioSource.isPlaying)
        {
            voiceAudioSource.Stop();
        }

        voiceAudioSource.clip = clip;
        voiceAudioSource.Play();
    }
}
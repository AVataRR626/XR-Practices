using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum SoundType
{
    Dialogue,
    BKSpeech,
    Scene,
    Alarms,
    Tones
}

[RequireComponent(typeof(AudioSource))]

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    private void Awake()
    {
        instance = this;
    }

    
    [Header("Audio Sources")]
    [Tooltip("Drag in the gameobjects from the hierachy with a sound source component attached. Sources should also have mixers attached to sound source output.")]
    public AudioSource dialogueSource;
    public AudioSource backgroundSpeechSource;
    public AudioSource sceneSoundsSource;
    public AudioSource alarmsSource;
    public AudioSource focusToneSource;



    [Header("Audio Clips")]
    [SerializeField] public SoundList[] soundList;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
   // audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
[Serializable]
public struct SoundList
{
    [SerializeField] private string name;
    [SerializeField] private AudioClip[] sounds;
}


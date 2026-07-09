using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;

public enum SoundType
{
    Dialogue,
    BackgroundSpeech,
    SceneSounds,
    Alarms,
    FocusTones
}


public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    private AudioSource audioSource;

    private void Awake() // Makes this is the only instance of the audio manager happening
    {
        instance = this;
    }

  
    [Header("Audio Clips")]
    [Tooltip("This is a list of the audio clips for these types of sounds.")]
    [SerializeField] public SoundList[] soundList;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
  
    }
    public void PlayAtAttach()
    {
        //audioSource = GetComponent<AudioSource>();
        

    }
         
 }


[Serializable]
public struct SoundList
{
    [Tooltip("Fill this in to find the clips more easily. Best to match the Sound Type. Else, will be Element 0, Element 1, ...")]
    [SerializeField] private string listName;
    [Tooltip("Drop down menu of existing categories of sounds.")] // Drop down uses public enum SoundType at the lines 7-14 of code
    [SerializeField] public SoundType soundTypeSelect;
    [Tooltip("Attach the audio mixer that controls that type of sound.")]
    [SerializeField] public AudioMixer audioMixer;
    [Tooltip("Add audio clips here to organise.")]
    [SerializeField] private AudioClip[] sounds;
}

 // [RequireComponent(typeof(AudioSource))]



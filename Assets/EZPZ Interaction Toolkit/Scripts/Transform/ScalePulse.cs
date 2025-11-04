/*BounceBlaster Project
QSI Util
ScalePulse.cs

Can be used for beating hearts, juciness effects for touch, etc.

-Matt Cabanag; 2 Oct 2016
*/

using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Rendering;

public class ScalePulse : MonoBehaviour
{
    public float pulseTime = 0.25f;
    public Vector3 scaleFactor = new Vector3(0.15f, 0.15f, 1);
    public Vector3 scaleAdd;
    public bool pulseOnStart = false;
    public float startDelay = 2;
    public bool pulseRepeat = false;
    public float pulseInterval = 3;
    //
    public AudioClip hoverEnterSound;
    public AudioClip hoverExitSound;
    public AudioClip clickSound;
    public AudioSource audioSource;

    private Vector3 originalScale;
    private float pulseClock = 0;   
    private bool pulseSwitch = false;
    private float timeLimit;
    private bool resetSwitch = false;

    //
    public ButtonType buttonType = ButtonType.Normal;
    public Volume volume;
    //
    public Action OnHover;
    public Action OnHoverExit;

	// Use this for initialization
	void Start ()
    {
        originalScale = transform.localScale;

        Reset();
    }

    void OnEnable()
    {
        if (pulseOnStart)
            Invoke("Pulse", startDelay);

        if (pulseRepeat)
            InvokeRepeating("Pulse", startDelay, pulseInterval);
    }

    void Reset()
    {
        pulseClock = 0;
        pulseSwitch = false;
        resetSwitch = false;
        //timeLimit = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(pulseSwitch)
        { 
            if (pulseClock < timeLimit)
            {
                pulseClock += Time.deltaTime;

                float pulseRatio = pulseClock / pulseTime;

                scaleAdd = scaleFactor;
                scaleAdd *= Mathf.Sin(Mathf.Deg2Rad * (pulseRatio * 180));

                transform.localScale = originalScale + scaleAdd;
            }
            else
            {
                pulseSwitch = false;
            }
        }
        else
        {
            if (!resetSwitch)
            {
                transform.localScale = originalScale;
                resetSwitch = true;
            }
        }
    }
    /// <summary>
    /// HoverEnter
    /// </summary>
    public void PulseUp()
    {
        //Debug.Log("PulseUp");
        pulseClock = 0;
        timeLimit = pulseTime / 2;
        pulseSwitch = true;
        resetSwitch = true;

        //根据不同类型执行逻辑
        switch (buttonType)
        {
            case ButtonType.Normal:
                break;
            case ButtonType.Sound:
                if (audioSource && hoverEnterSound)
                    audioSource.PlayOneShot(hoverEnterSound);
                break;
            case ButtonType.Vision:
                OnHover?.Invoke();
                break;
            case ButtonType.Immersive:
                if (audioSource && hoverEnterSound)
                    audioSource.PlayOneShot(hoverEnterSound);
                volume.gameObject.SetActive(true);
                break;
        }
       

    }
    /// <summary>
    /// HoverExit
    /// </summary>
    public void PulseDown()
    {

        pulseClock = pulseTime / 2;
        timeLimit = pulseTime;
        pulseSwitch = true;
        resetSwitch = false;

        if (audioSource && hoverExitSound)
            audioSource.PlayOneShot(hoverExitSound);

        volume.gameObject.SetActive(false);
        OnHoverExit?.Invoke();
    }

    /// <summary>
    /// MouseDown
    /// </summary>
    [ContextMenu("Pulse")]
    public void Pulse()
    {
        pulseClock = 0;
        timeLimit = pulseTime;
        pulseSwitch = true;
        resetSwitch = false;
        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound);
    }

    /// <summary>
    /// Button Types Enum
    /// </summary>
    public enum ButtonType
    { 
        Normal,
        Sound,
        Vision,
        Immersive
    }
}

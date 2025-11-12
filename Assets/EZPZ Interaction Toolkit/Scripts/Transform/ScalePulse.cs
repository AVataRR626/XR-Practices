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
using System.Collections.Generic;
using TMPro;

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


    //button switch
    public bool _switch = false;
    public List<ButtonType> btnEffects =new List<ButtonType>();
    public TextMeshProUGUI _SoundTxt;
    public TextMeshProUGUI _VisionTxt;
    public TextMeshProUGUI _ImmersiveTxt;

    // Use this for initialization
    void Start ()
    {
        originalScale = transform.localScale;

        Reset();
        SetCurrModeText(ButtonType.Normal);
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

        // sound effect
        if (btnEffects.Contains(ButtonType.Sound))
        {
            if (audioSource && hoverEnterSound)
                audioSource.PlayOneShot(hoverEnterSound);
        }
        // vision effect
        if (btnEffects.Contains(ButtonType.Vision))
        {
            OnHover?.Invoke();
        }
        // immersive effect
        if (btnEffects.Contains(ButtonType.Immersive))
        {
            volume.gameObject.SetActive(true);
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

        // sound effect
        if (btnEffects.Contains(ButtonType.Sound))
        {
            if (audioSource && hoverExitSound)
                audioSource.PlayOneShot(hoverExitSound);
        }
        // vision effect
        if (btnEffects.Contains(ButtonType.Vision))
        {
            OnHoverExit?.Invoke();
        }
        // immersive effect
        if (btnEffects.Contains(ButtonType.Immersive))
        {
            volume?.gameObject.SetActive(false);
        }
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

        // sound effect
        if (btnEffects.Contains(ButtonType.Sound))
        {
            if (audioSource && clickSound)
                audioSource.PlayOneShot(clickSound);
        }
        // vision effect
        if (btnEffects.Contains(ButtonType.Vision))
        {
            OnHoverExit?.Invoke();
        }
        // immersive effect
        if (btnEffects.Contains(ButtonType.Immersive))
        {
            //volume.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Set Button Type
    /// </summary>
    /// <param name="type"></param>
    public void SetButtonType(string type)
    {
        Debug.Log("按下");
        ButtonType target = Enum.Parse<ButtonType>(type);
        if (target == ButtonType.Normal)
        {
            btnEffects.Clear();
            SetCurrModeText(target);
            return;
        }
        if (btnEffects.Count > 0 && btnEffects.Contains(target))
        {
            btnEffects.Remove(target);

            Debug.Log("$移除{target}");
        }
        else
        {
            btnEffects.Add(target); Debug.Log("$添加{target}");
        }
        SetCurrModeText(target);
    }

    public void SetCurrModeText(ButtonType target)
    {
        switch (target)
        {
            case ButtonType.Normal:
                if (!_SoundTxt || !_VisionTxt||!_ImmersiveTxt)
                    break;
                _SoundTxt.text = $"Sound : Deactive";
                _VisionTxt.text = $"Vision : Deactive";
                _ImmersiveTxt.text = $"Immersive : Deactive";
                break;
            case ButtonType.Sound:
                if (!_SoundTxt)
                    break;
                if (btnEffects.Contains(target))
                    _SoundTxt.text = $"Sound : Activate";
                else
                    _SoundTxt.text = $"Sound : Deactive";
                break;
            case ButtonType.Vision:
                if (!_VisionTxt)
                    break;
                if (btnEffects.Contains(target))
                    _VisionTxt.text = $"Vision : Activate";
                else
                    _VisionTxt.text = $"Vision : Deactive";
                break;
            case ButtonType.Immersive:
                if (!_ImmersiveTxt)
                    break;
                if (btnEffects.Contains(target))
                    _ImmersiveTxt.text = $"Immersive : Activate";
                else
                    _ImmersiveTxt.text = $"Immersive : Deactive";
                break;
        }


    }
    public void SetButtonStyle()
    {
       
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

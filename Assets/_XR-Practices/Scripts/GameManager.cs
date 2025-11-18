using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button[] _MainButton;

    public List<ButtonType> btnEffects = new List<ButtonType>();
    public Text _InfoTxt;

    //
    //public AnimationCurve bounceCurve;
    public Volume volume;
    public AudioClip hoverEnterSound;
    public AudioClip hoverExitSound;
    public AudioClip clickSound;
    public AudioSource audioSource;

    //private bool isBouncing = false;
    public GameObject currButton;


    private void Start()
    {
        SetCurrModeText(ButtonType.Normal);
    }
    /// <summary>
    ///按钮悬停效果
    /// </summary>
    public void OnButtonHoverEnter(GameObject target)
    {
        if (currButton == target) return;
        currButton = target;
        // sound effect
        if (btnEffects.Contains(ButtonType.Sound))  
        {
            if (audioSource && hoverEnterSound)
                audioSource.PlayOneShot(hoverEnterSound);
        }
        // vision effect
        if (btnEffects.Contains(ButtonType.Vision))
        {
            //if (!isBouncing)
            Animator anim = currButton.GetComponent<Animator>();
            if (anim != null)
                PlayerButtonAnimation(anim, true);
        }
        // immersive effect
        if (btnEffects.Contains(ButtonType.Immersive))
        {
            volume.gameObject.SetActive(true);
        }
    }
    public void OnButtonHoverExitr(GameObject target)
    {
      
        // sound effect
        if (btnEffects.Contains(ButtonType.Sound))
        {
            if (audioSource && hoverExitSound)
                audioSource.PlayOneShot(hoverExitSound);
        }
        // vision effect
        if (btnEffects.Contains(ButtonType.Vision))
        {
            //OnHoverExit?.Invoke();
            //currButton.transform.localScale = new Vector3(1, 1, 1);
            if (currButton)
            {
                Animator anim = currButton.GetComponent<Animator>();
                if (anim != null)
                    PlayerButtonAnimation(anim, false);
            }
            //isBouncing = false;
        }
        // immersive effect
        if (btnEffects.Contains(ButtonType.Immersive))
        {
            volume?.gameObject.SetActive(false);
        }
        if (currButton == target)
        {
            currButton = null;
        }
    }
    public void OnButtonClick()
    {
        // sound effect
        if (btnEffects.Contains(ButtonType.Sound))
        {
            if (audioSource && clickSound)
                audioSource.PlayOneShot(clickSound);
        }
        // vision effect
        if (btnEffects.Contains(ButtonType.Vision))
        {
            //OnHoverExit?.Invoke();
        }
        // immersive effect
        if (btnEffects.Contains(ButtonType.Immersive))
        {
            //volume.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 添加按钮类型到管理员
    /// </summary>
    /// <param name="name"></param>
    public void AddButtonType(string name)
    {
        ButtonType target = Enum.Parse<ButtonType>(name);
        if (target == ButtonType.Normal)   // normal type  clear all types
        {
            btnEffects.Clear();
            SetCurrModeText(target);
            return;
        }
        if (btnEffects.Count > 0 && btnEffects.Contains(target))
        {
            btnEffects.Remove(target);   // already have this type, so remove it
            Debug.Log("$移除{target}");
        }
        else   // add this type
        {
            btnEffects.Add(target); Debug.Log("$添加{target}");
        }
        SetCurrModeText(target);
    }
    /// <summary>
    /// 设置主按钮颜色
    /// </summary>
    /// <param name="button"></param>
    public void SetColor(Button button)
    {
        if (_MainButton == null || _MainButton.Length<1) return;
        foreach (Button btn in _MainButton)
        {
            btn.colors = button.colors;
        }
    }
    public void SetHoverVoice(AudioClip audioClip)
    {
        hoverEnterSound = audioClip;
    }

    /// <summary>
    /// 设置按钮类型提示文本
    /// </summary>
    /// <param name="target"></param>
    public void SetCurrModeText(ButtonType target)
    {
        string msg = "";
        switch (target)
        {
            case ButtonType.Normal:
                if (!_InfoTxt)
                    break;
                msg = "Standard button behavior";
                volume.gameObject.SetActive(false);
                break;

            default:
                if(btnEffects.Contains(ButtonType.Sound))
                    msg = "Audio feedback enabledc";
                if(btnEffects.Contains(ButtonType.Vision))
                    msg += "\nButton flash on hoverc";
                if(btnEffects.Contains(ButtonType.Immersive))
                    msg += "\nImmersive mode enabledc";
                break;
        }
        _InfoTxt.text = msg;
    }

    //public void HandleHover(GameObject target)
    //{
    //    Debug.Log("Hover Enter");
    //    if (!btnEffects.Contains(ButtonType.Vision)) return;
    //    if (currButton == target) return;
    //    currButton = target;
    //    if (!isBouncing)
    //        StartCoroutine(BounceCoroutine());
    //}
    //public void HandleHoverExit(GameObject target)
    //{
    //    Debug.Log("Hover Exit");
    //    if (currButton == target)
    //    {
    //        currButton.transform.localScale = new Vector3(1, 1, 1);
    //        currButton = null;
    //    }
    //    isBouncing = false;
    //    //buttonTransform.localScale = defaultScale;
    //}
    private void PlayerButtonAnimation(Animator animator,bool val)
    {
        animator.SetBool("Hover", val);
    }
    /// <summary>
    /// 弹跳动画 分步执行  （根据动画曲线参数）
    /// </summary>
    /// <returns></returns>
    //private IEnumerator BounceCoroutine()
    //{
    //    yield return new WaitForNextFrameUnit();
    //    isBouncing = true;
    //    Vector3 originalScale = new Vector3(1, 1, 1);
    //    while (isBouncing)
    //    {
    //        float elapsedTime = 0f;
    //        float duration = 0.5f; // Duration of the bounce
    //        while (elapsedTime < duration && isBouncing)
    //        {
    //            float t = elapsedTime / duration;
    //            float scaleMultiplier = bounceCurve.Evaluate(t);
    //            Vector3 targetScale = originalScale * scaleMultiplier;
    //            targetScale.z = originalScale.z;
    //            if (currButton)
    //                currButton.transform.localScale = targetScale;
    //            elapsedTime += Time.deltaTime;
    //            yield return null;
    //        }
    //        if(currButton)
    //            currButton.transform.localScale = originalScale;
    //        //Wait for a short period before the next bounce
    //        if (isBouncing)
    //                yield return new WaitForSeconds(0.5f);
    //    }
    //    //currButton.transform.localScale = originalScale;
    //}
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

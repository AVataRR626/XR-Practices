using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonBounce : MonoBehaviour
{
    public AnimationCurve bounceCurve;

    ScalePulse scalePulse;
    private bool isBouncing = false;
    private Transform buttonTransform;
    Vector3 defaultScale;
    private void Start()
    {
        scalePulse = GetComponent<ScalePulse>();
        buttonTransform = transform.Find("Graphics");
        defaultScale = buttonTransform.localScale;
        if (scalePulse)
        {
            scalePulse.OnHover+= HandleHover;
            scalePulse.OnHoverExit+= HandleHoverExit;
        }
    }

    private void HandleHoverExit()
    {
        Debug.Log("Hover Exit");
        isBouncing = false;
        buttonTransform.localScale = defaultScale;
    }

    private void HandleHover()
    {
        Debug.Log("Hover Enter");
        StartCoroutine(BounceCoroutine());
    }


    private IEnumerator BounceCoroutine()
    {
        yield return new WaitForNextFrameUnit();
        isBouncing = true;
        while (isBouncing)
        {

            scalePulse.audioSource.PlayOneShot(scalePulse.hoverEnterSound);
            float elapsedTime = 0f;
            float duration = 0.5f; // Duration of the bounce
            Vector3 originalScale = defaultScale;
            while (elapsedTime < duration && isBouncing)
            {
                float t = elapsedTime / duration;
                float scaleMultiplier = bounceCurve.Evaluate(t);
                buttonTransform.localScale = originalScale * scaleMultiplier;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            buttonTransform.localScale = originalScale;
            // Wait for a short period before the next bounce
            if(isBouncing)
                yield return new WaitForSeconds(0.5f);
        }
        buttonTransform.localScale = defaultScale;
    }
}

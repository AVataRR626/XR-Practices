using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BadSceneTimeSelectorController : MonoBehaviour
{
    [Header("Core Interaction")]
    [SerializeField] private XRSocketInteractor deviceSocket;
    [SerializeField] private XRGrabInteractable reagentBeaker;
    [SerializeField] private XRSimpleInteractable timeSelectorInteractable;

    [Header("Knob Rotation")]
    [SerializeField] private Transform timeSelectorPivot;
    [SerializeField] private Vector3 rotationAxis = new Vector3(0f, 1f, 0f);

    [Header("Pour Lid")]
    [SerializeField] private Transform pourLidPivot;
    [SerializeField] private Vector3 lidClosedLocalEuler = new Vector3(0f, 0f, 0f);
    [SerializeField] private float lidCloseDelay = 0.08f;
    [SerializeField] private float lidCloseDuration = 0.35f;

    [Header("Timing")]
    [SerializeField] private float analysisDelay = 2f;

    [Header("Computer Screen")]
    [SerializeField] private Image screenPanel;
    [SerializeField] private Color screenIdleColor = new Color(0.12f, 0.12f, 0.12f, 0.95f);

    [Header("Warning Lamp")]
    [SerializeField] private Renderer lampBulbRenderer;
    [SerializeField] private Material lampIdleMaterial;
    [SerializeField] private Material lampAlertMaterial;
    [SerializeField] private Material lampSuccessMaterial;
    [SerializeField] private Light lampPointLight;
    [SerializeField] private float lampFlashDuration = 1.2f;
    [SerializeField] private float lampFlashInterval = 0.18f;
    [SerializeField] private float lampSuccessHoldTime = 2f;
    [SerializeField] private Color lampAlertLightColor = Color.red;
    [SerializeField] private Color lampSuccessLightColor = Color.green;

    [Header("Audio")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip pourClip;
    [SerializeField] private AudioClip analyzingClip;
    [SerializeField] private AudioClip errorClip;
    [SerializeField] private AudioClip completeClip;
    [SerializeField] private AudioClip invalidPlacementClip;

    private bool reagentPlaced = false;
    private bool reagentConsumed = false;
    private bool isAnalyzing = false;

    private int currentStage = 0;
    private Quaternion initialKnobRotation;
    private Coroutine analysisCoroutine;
    private Coroutine releaseCheckCoroutine;
    private Coroutine lampFlashCoroutine;
    private Coroutine lampSuccessCoroutine;
    private Coroutine lidMoveCoroutine;

    private void Start()
    {
        if (timeSelectorPivot != null)
        {
            initialKnobRotation = timeSelectorPivot.localRotation;
        }

        SetIdleState();
    }

    private void OnEnable()
    {
        if (deviceSocket != null)
        {
            deviceSocket.selectEntered.AddListener(OnSocketEntered);
        }

        if (timeSelectorInteractable != null)
        {
            timeSelectorInteractable.selectEntered.AddListener(OnTimeSelectorSelected);
        }

        if (reagentBeaker != null)
        {
            reagentBeaker.selectExited.AddListener(OnBeakerReleased);
        }
    }

    private void OnDisable()
    {
        if (deviceSocket != null)
        {
            deviceSocket.selectEntered.RemoveListener(OnSocketEntered);
        }

        if (timeSelectorInteractable != null)
        {
            timeSelectorInteractable.selectEntered.RemoveListener(OnTimeSelectorSelected);
        }

        if (reagentBeaker != null)
        {
            reagentBeaker.selectExited.RemoveListener(OnBeakerReleased);
        }
    }

    private void OnSocketEntered(SelectEnterEventArgs args)
    {
        if (args == null || args.interactableObject == null) return;
        if (reagentBeaker == null) return;
        if (args.interactableObject.transform != reagentBeaker.transform) return;

        reagentPlaced = true;
        reagentConsumed = true;
        currentStage = 0;

        StopAnalysisIfRunning();
        StopLampCoroutines();
        ResetKnobRotation();
        SetIdleVisualState();
        SetLampIdle();
        PlayClip(pourClip);
        ClosePourLid();
    }

    private void OnBeakerReleased(SelectExitEventArgs args)
    {
        if (args == null || args.interactableObject == null) return;
        if (reagentBeaker == null) return;
        if (args.interactableObject.transform != reagentBeaker.transform) return;

        if (releaseCheckCoroutine != null)
        {
            StopCoroutine(releaseCheckCoroutine);
        }

        releaseCheckCoroutine = StartCoroutine(DelayedReleaseCheck());
    }

    private IEnumerator DelayedReleaseCheck()
    {
        yield return new WaitForSeconds(0.12f);

        if (!reagentPlaced && !reagentConsumed)
        {
            TriggerWeakWarning(invalidPlacementClip);
        }
    }

    private void OnTimeSelectorSelected(SelectEnterEventArgs args)
    {
        AdvanceTimeSelection();
    }

    public void AdvanceTimeSelection()
    {
        if (isAnalyzing)
            return;

        if (!reagentPlaced)
        {
            TriggerWeakWarning(invalidPlacementClip);
            return;
        }

        currentStage++;

        if (currentStage > 3)
        {
            currentStage = 0;
        }

        RotateKnobToStage(currentStage);

        if (currentStage == 0)
        {
            SetIdleVisualState();
            SetLampIdle();
        }
        else
        {
            StartAnalysisForStage(currentStage);
        }
    }

    private void StartAnalysisForStage(int stage)
    {
        StopAnalysisIfRunning();
        analysisCoroutine = StartCoroutine(AnalysisRoutine(stage));
    }

    private IEnumerator AnalysisRoutine(int stage)
    {
        isAnalyzing = true;

        SetIdleVisualState();
        SetLampIdle();
        PlayClip(analyzingClip);

        yield return new WaitForSeconds(analysisDelay);

        isAnalyzing = false;
        analysisCoroutine = null;

        ApplyFinalStageFeedback(stage);
    }

    private void StopAnalysisIfRunning()
    {
        if (analysisCoroutine != null)
        {
            StopCoroutine(analysisCoroutine);
            analysisCoroutine = null;
        }

        isAnalyzing = false;
    }

    private void StopLampCoroutines()
    {
        if (lampFlashCoroutine != null)
        {
            StopCoroutine(lampFlashCoroutine);
            lampFlashCoroutine = null;
        }

        if (lampSuccessCoroutine != null)
        {
            StopCoroutine(lampSuccessCoroutine);
            lampSuccessCoroutine = null;
        }
    }

    private void ResetKnobRotation()
    {
        if (timeSelectorPivot != null)
        {
            timeSelectorPivot.localRotation = initialKnobRotation;
        }
    }

    private void RotateKnobToStage(int stage)
    {
        if (timeSelectorPivot == null) return;

        float angle = 0f;

        switch (stage)
        {
            case 0:
                angle = 0f;
                break;
            case 1:
                angle = 90f;
                break;
            case 2:
                angle = 180f;
                break;
            case 3:
                angle = 270f;
                break;
        }

        Quaternion targetRotation =
            initialKnobRotation *
            Quaternion.AngleAxis(angle, rotationAxis.normalized);

        timeSelectorPivot.localRotation = targetRotation;
    }

    private void SetIdleState()
    {
        SetIdleVisualState();
        SetLampIdle();
    }

    private void SetIdleVisualState()
    {
        if (screenPanel != null)
        {
            screenPanel.color = screenIdleColor;
        }
    }

    private void ApplyFinalStageFeedback(int stage)
    {
        switch (stage)
        {
            case 1:
                TriggerWeakWarning(errorClip);
                break;

            case 2:
                TriggerSuccessLamp();
                PlayClip(completeClip);
                break;

            case 3:
                TriggerWeakWarning(errorClip);
                break;
        }
    }

    private void TriggerWeakWarning(AudioClip clip)
    {
        PlayClip(clip);
        FlashLamp();
    }

    private void TriggerSuccessLamp()
    {
        StopLampCoroutines();
        lampSuccessCoroutine = StartCoroutine(SuccessLampRoutine());
    }

    private void FlashLamp()
    {
        StopLampCoroutines();
        lampFlashCoroutine = StartCoroutine(LampFlashRoutine());
    }

    private IEnumerator LampFlashRoutine()
    {
        float elapsed = 0f;
        bool isOn = false;

        while (elapsed < lampFlashDuration)
        {
            isOn = !isOn;
            SetLampAlertVisual(isOn);
            yield return new WaitForSeconds(lampFlashInterval);
            elapsed += lampFlashInterval;
        }

        SetLampIdle();
        lampFlashCoroutine = null;
    }

    private IEnumerator SuccessLampRoutine()
    {
        SetLampSuccessVisual(true);
        yield return new WaitForSeconds(lampSuccessHoldTime);
        SetLampIdle();
        lampSuccessCoroutine = null;
    }

    private void SetLampAlertVisual(bool alertOn)
    {
        if (lampBulbRenderer != null)
        {
            lampBulbRenderer.material = alertOn ? lampAlertMaterial : lampIdleMaterial;
        }

        if (lampPointLight != null)
        {
            lampPointLight.color = lampAlertLightColor;
            lampPointLight.enabled = alertOn;
        }
    }

    private void SetLampSuccessVisual(bool successOn)
    {
        if (lampBulbRenderer != null)
        {
            lampBulbRenderer.material = successOn ? lampSuccessMaterial : lampIdleMaterial;
        }

        if (lampPointLight != null)
        {
            lampPointLight.color = lampSuccessLightColor;
            lampPointLight.enabled = successOn;
        }
    }

    private void SetLampIdle()
    {
        if (lampBulbRenderer != null && lampIdleMaterial != null)
        {
            lampBulbRenderer.material = lampIdleMaterial;
        }

        if (lampPointLight != null)
        {
            lampPointLight.enabled = false;
        }
    }

    private void ClosePourLid()
    {
        if (pourLidPivot == null) return;

        if (lidMoveCoroutine != null)
        {
            StopCoroutine(lidMoveCoroutine);
        }

        lidMoveCoroutine = StartCoroutine(ClosePourLidRoutine());
    }

    private IEnumerator ClosePourLidRoutine()
    {
        if (lidCloseDelay > 0f)
        {
            yield return new WaitForSeconds(lidCloseDelay);
        }

        Quaternion startRotation = pourLidPivot.localRotation;
        Quaternion targetRotation = Quaternion.Euler(lidClosedLocalEuler);

        float elapsed = 0f;

        while (elapsed < lidCloseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lidCloseDuration);
            t = t * t * (3f - 2f * t);

            pourLidPivot.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        pourLidPivot.localRotation = targetRotation;
        lidMoveCoroutine = null;
    }

    private void PlayClip(AudioClip clip)
    {
        if (uiAudioSource == null || clip == null) return;
        uiAudioSource.PlayOneShot(clip);
    }
}
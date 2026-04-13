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
    [SerializeField] private float rotationStep = 90f;

    [Header("Timing")]
    [SerializeField] private float analysisDelay = 2f;

    [Header("Computer Screen")]
    [SerializeField] private Image screenPanel;
    [SerializeField] private Color screenIdleColor = new Color(0.12f, 0.12f, 0.12f, 0.95f);

    [Header("Warning Light")]
    [SerializeField] private GameObject warningLight;
    [SerializeField] private float warningLightOnTime = 1.2f;

    [Header("Audio")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip reagentDetectedClip;
    [SerializeField] private AudioClip analyzingClip;
    [SerializeField] private AudioClip errorClip;
    [SerializeField] private AudioClip completeClip;
    [SerializeField] private AudioClip invalidPlacementClip;

    private bool reagentPlaced = false;
    private bool isAnalyzing = false;
    private bool isComplete = false;

    private int currentStage = 0;
    // 0 = no time selected
    // 1 = 15 s
    // 2 = 30 s
    // 3 = 45 s

    private Quaternion initialKnobRotation;
    private Coroutine analysisCoroutine;
    private Coroutine warningLightCoroutine;

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
            deviceSocket.selectExited.AddListener(OnSocketExited);
        }

        if (timeSelectorInteractable != null)
        {
            timeSelectorInteractable.selectEntered.AddListener(OnTimeSelectorSelected);
        }
    }

    private void OnDisable()
    {
        if (deviceSocket != null)
        {
            deviceSocket.selectEntered.RemoveListener(OnSocketEntered);
            deviceSocket.selectExited.RemoveListener(OnSocketExited);
        }

        if (timeSelectorInteractable != null)
        {
            timeSelectorInteractable.selectEntered.RemoveListener(OnTimeSelectorSelected);
        }
    }

    private void OnSocketEntered(SelectEnterEventArgs args)
    {
        if (args == null || args.interactableObject == null) return;
        if (reagentBeaker == null) return;
        if (args.interactableObject.transform != reagentBeaker.transform) return;

        reagentPlaced = true;
        isComplete = false;
        currentStage = 0;

        StopAnalysisIfRunning();
        ResetKnobRotation();
        SetIdleVisualState();
        PlayClip(reagentDetectedClip);
    }

    private void OnSocketExited(SelectExitEventArgs args)
    {
        if (args == null || args.interactableObject == null) return;
        if (reagentBeaker == null) return;
        if (args.interactableObject.transform != reagentBeaker.transform) return;

        reagentPlaced = false;
        currentStage = 0;
        isComplete = false;

        StopAnalysisIfRunning();
        ResetKnobRotation();
        SetIdleState();
    }

    private void OnTimeSelectorSelected(SelectEnterEventArgs args)
    {
        AdvanceTimeSelection();
    }

    public void AdvanceTimeSelection()
    {
        if (isAnalyzing)
        {
            Debug.Log("Analysis in progress.");
            return;
        }

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
        TurnOffWarningLight();
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
        isComplete = false;

        switch (stage)
        {
            case 1:
                TriggerWeakWarning(errorClip);
                break;

            case 2:
                isComplete = true;
                TurnOffWarningLight();
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
        FlashWarningLight();
    }

    private void FlashWarningLight()
    {
        if (warningLight == null) return;

        if (warningLightCoroutine != null)
        {
            StopCoroutine(warningLightCoroutine);
        }

        warningLightCoroutine = StartCoroutine(WarningLightRoutine());
    }

    private IEnumerator WarningLightRoutine()
    {
        warningLight.SetActive(true);
        yield return new WaitForSeconds(warningLightOnTime);
        warningLight.SetActive(false);
        warningLightCoroutine = null;
    }

    private void TurnOffWarningLight()
    {
        if (warningLight != null)
        {
            warningLight.SetActive(false);
        }

        if (warningLightCoroutine != null)
        {
            StopCoroutine(warningLightCoroutine);
            warningLightCoroutine = null;
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (uiAudioSource == null || clip == null) return;
        uiAudioSource.PlayOneShot(clip);
    }
}
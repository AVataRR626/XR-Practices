using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DetectionTimeSelectorController : MonoBehaviour
{
    [Header("Core Interaction")]
    [SerializeField] private XRSocketInteractor deviceSocket;
    [SerializeField] private XRGrabInteractable reagentBeaker;
    [SerializeField] private XRSimpleInteractable timeSelectorInteractable;

    [Header("Knob Rotation")]
    [SerializeField] private Transform timeSelectorPivot;
    [SerializeField] private Vector3 rotationAxis = new Vector3(0f, 1f, 0f);
    [SerializeField] private float rotationStep = 90f;

    [Header("Pour Lid")]
    [SerializeField] private Transform pourLidPivot;
    [SerializeField] private Vector3 lidClosedLocalEuler = new Vector3(0f, 0f, 0f);
    [SerializeField] private float lidCloseDelay = 0.08f;
    [SerializeField] private float lidCloseDuration = 0.35f;

    [Header("Timing")]
    [SerializeField] private float analysisDelay = 2f;

    [Header("Device UI")]
    [SerializeField] private TMP_Text durationText;
    [SerializeField] private TMP_Text deviceStateText;

    [Header("Computer UI")]
    [SerializeField] private Image screenPanel;
    [SerializeField] private TMP_Text statusText;

    [Header("Whiteboard Instructor")]
    [SerializeField] private TMP_Text instructorMessageText;
    [SerializeField] private LabInstructorVoiceController instructorVoiceController;
    [SerializeField] private float instructorVoiceDelayAfterFeedback = 0.45f;

    [Header("Optional Label Highlights")]
    [SerializeField] private TMP_Text label15s;
    [SerializeField] private TMP_Text label30s;
    [SerializeField] private TMP_Text label45s;
    [SerializeField] private Color labelNormalColor = new Color(0.85f, 0.85f, 0.85f, 1f);
    [SerializeField] private Color labelSelectedColor = new Color(1f, 0.85f, 0.25f, 1f);

    [Header("Screen Colors")]
    [SerializeField] private Color standbyColor = new Color(0.08f, 0.08f, 0.08f, 0.95f);
    [SerializeField] private Color reagentDetectedColor = new Color(0.90f, 0.75f, 0.20f, 0.95f);
    [SerializeField] private Color analyzingColor = new Color(0.15f, 0.65f, 0.65f, 0.95f);
    [SerializeField] private Color tooShortColor = new Color(0.20f, 0.45f, 0.90f, 0.95f);
    [SerializeField] private Color completeColor = new Color(0.18f, 0.65f, 0.25f, 0.95f);
    [SerializeField] private Color tooLongColor = new Color(0.78f, 0.20f, 0.20f, 0.95f);

    [Header("Status Messages")]
    [SerializeField] private string standbyMessage = "Device standby.";
    [SerializeField] private string reagentDetectedMessage = "Reagent loaded. Select reaction time.";
    [SerializeField] private string analyzingMessage = "Analyzing...";
    [SerializeField] private string tooShortMessage = "Reaction time too short. Please try again.";
    [SerializeField] private string completeMessage = "Reaction complete.";
    [SerializeField] private string tooLongMessage = "Reaction time too long. Please try again.";

    [Header("Instructor Text")]
    [SerializeField] private string introInstruction = "Step 1: Pick up the beaker and pour the reagent into the device.";
    [SerializeField] private string invalidPlacementInstruction = "The reagent was not poured into the device. Place the beaker over the opening and try again.";
    [SerializeField] private string detectedInstruction = "Step 2: Turn the time selector to set the reaction time.";
    [SerializeField] private string analyzingInstruction = "The device is analyzing the sample.";
    [SerializeField] private string tooShortInstruction = "Warning: The reaction time is too short. Increase the duration.";
    [SerializeField] private string completeInstruction = "Well done. The reaction is complete.";
    [SerializeField] private string tooLongInstruction = "Warning: The reaction time is too long. Reduce the duration.";

    [Header("Audio")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip pourClip;
    [SerializeField] private AudioClip analyzingClip;
    [SerializeField] private AudioClip tooShortClip;
    [SerializeField] private AudioClip completeClip;
    [SerializeField] private AudioClip tooLongClip;
    [SerializeField] private AudioClip invalidPlacementClip;

    [Header("Haptic Settings")]
    [SerializeField, Range(0f, 1f)] private float invalidPlacementAmplitude = 0.45f;
    [SerializeField] private float invalidPlacementDuration = 0.12f;

    [SerializeField, Range(0f, 1f)] private float tooShortAmplitude = 0.45f;
    [SerializeField] private float tooShortPulseDuration = 0.10f;
    [SerializeField] private float tooShortGap = 0.08f;

    [SerializeField, Range(0f, 1f)] private float completeAmplitude = 0.35f;
    [SerializeField] private float completeDuration = 0.18f;

    [SerializeField, Range(0f, 1f)] private float tooLongAmplitude = 0.60f;
    [SerializeField] private float tooLongPulseDuration = 0.18f;
    [SerializeField] private float tooLongGap = 0.10f;

    private bool reagentPlaced = false;
    private bool reagentConsumed = false;
    private bool isAnalyzing = false;
    private bool isComplete = false;

    private int currentStage = 0;
    // 0 = reagent loaded, no time selected yet
    // 1 = 15 s
    // 2 = 30 s
    // 3 = 45 s

    private Quaternion initialKnobRotation;
    private Coroutine analysisCoroutine;
    private Coroutine releaseCheckCoroutine;
    private Coroutine hapticRoutine;
    private Coroutine lidMoveCoroutine;

    private void Start()
    {
        if (timeSelectorPivot != null)
        {
            initialKnobRotation = timeSelectorPivot.localRotation;
        }

        SetStandbyState();
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
            reagentBeaker.selectEntered.AddListener(OnBeakerGrabbed);
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
            reagentBeaker.selectEntered.RemoveListener(OnBeakerGrabbed);
            reagentBeaker.selectExited.RemoveListener(OnBeakerReleased);
        }
    }

    private void OnBeakerGrabbed(SelectEnterEventArgs args)
    {
        if (args == null || args.interactableObject == null) return;
        if (reagentBeaker == null) return;
        if (args.interactableObject.transform != reagentBeaker.transform) return;

        if (instructorVoiceController != null)
        {
            instructorVoiceController.PlayPickUpBeakerVoice();
        }
    }

    private void OnSocketEntered(SelectEnterEventArgs args)
    {
        if (args == null || args.interactableObject == null) return;
        if (reagentBeaker == null) return;
        if (args.interactableObject.transform != reagentBeaker.transform) return;

        reagentPlaced = true;
        reagentConsumed = true;
        isComplete = false;
        currentStage = 0;

        StopAnalysisIfRunning();
        ResetKnobRotation();
        SetReagentDetectedState();
        PlayClip(pourClip);
        ClosePourLid();

        if (instructorVoiceController != null)
        {
            instructorVoiceController.PlayReagentLoadedVoiceDelayed(instructorVoiceDelayAfterFeedback);
        }
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
            SetInstructorMessage(invalidPlacementInstruction);
            PlayClip(invalidPlacementClip);
            PlaySingleHaptics(invalidPlacementAmplitude, invalidPlacementDuration);

            if (instructorVoiceController != null)
            {
                instructorVoiceController.PlayInvalidPlacementVoiceDelayed(instructorVoiceDelayAfterFeedback);
            }
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
            SetInstructorMessage(invalidPlacementInstruction);
            PlayClip(invalidPlacementClip);
            PlaySingleHaptics(invalidPlacementAmplitude, invalidPlacementDuration);

            if (instructorVoiceController != null)
            {
                instructorVoiceController.PlayInvalidPlacementVoiceDelayed(instructorVoiceDelayAfterFeedback);
            }

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
            SetReagentDetectedState();

            if (instructorVoiceController != null)
            {
                instructorVoiceController.PlayReagentLoadedVoiceDelayed(instructorVoiceDelayAfterFeedback);
            }
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

        ApplyAnalyzingState(stage);
        PlayClip(analyzingClip);

        if (instructorVoiceController != null)
        {
            instructorVoiceController.PlayAnalyzingVoiceDelayed(instructorVoiceDelayAfterFeedback);
        }

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

    private void SetStandbyState()
    {
        if (durationText != null) durationText.text = "-- s";
        if (deviceStateText != null) deviceStateText.text = "Ready";

        if (screenPanel != null) screenPanel.color = standbyColor;
        if (statusText != null) statusText.text = standbyMessage;

        UpdateLabelHighlight(0);
        SetInstructorMessage(introInstruction);
    }

    private void SetReagentDetectedState()
    {
        if (durationText != null) durationText.text = "-- s";
        if (deviceStateText != null) deviceStateText.text = "Loaded";

        if (screenPanel != null) screenPanel.color = reagentDetectedColor;
        if (statusText != null) statusText.text = reagentDetectedMessage;

        UpdateLabelHighlight(0);
        SetInstructorMessage(detectedInstruction);
    }

    private void ApplyAnalyzingState(int stage)
    {
        switch (stage)
        {
            case 1:
                if (durationText != null) durationText.text = "15 s";
                break;

            case 2:
                if (durationText != null) durationText.text = "30 s";
                break;

            case 3:
                if (durationText != null) durationText.text = "45 s";
                break;
        }

        if (deviceStateText != null) deviceStateText.text = "Analyzing";

        if (screenPanel != null) screenPanel.color = analyzingColor;
        if (statusText != null) statusText.text = analyzingMessage;

        UpdateLabelHighlight(stage);
        SetInstructorMessage(analyzingInstruction);
    }

    private void ApplyFinalStageFeedback(int stage)
    {
        isComplete = false;

        switch (stage)
        {
            case 1:
                if (durationText != null) durationText.text = "15 s";
                if (deviceStateText != null) deviceStateText.text = "Too Short";
                if (screenPanel != null) screenPanel.color = tooShortColor;
                if (statusText != null) statusText.text = tooShortMessage;

                UpdateLabelHighlight(1);
                SetInstructorMessage(tooShortInstruction);
                PlayClip(tooShortClip);
                PlayDoubleHaptics(tooShortAmplitude, tooShortPulseDuration, tooShortGap);

                if (instructorVoiceController != null)
                {
                    instructorVoiceController.PlayTooShortVoiceDelayed(instructorVoiceDelayAfterFeedback);
                }

                break;

            case 2:
                isComplete = true;

                if (durationText != null) durationText.text = "30 s";
                if (deviceStateText != null) deviceStateText.text = "Complete";
                if (screenPanel != null) screenPanel.color = completeColor;
                if (statusText != null) statusText.text = completeMessage;

                UpdateLabelHighlight(2);
                SetInstructorMessage(completeInstruction);
                PlayClip(completeClip);
                PlaySingleHaptics(completeAmplitude, completeDuration);

                if (instructorVoiceController != null)
                {
                    instructorVoiceController.PlayCompleteVoiceDelayed(instructorVoiceDelayAfterFeedback);
                }

                break;

            case 3:
                if (durationText != null) durationText.text = "45 s";
                if (deviceStateText != null) deviceStateText.text = "Too Long";
                if (screenPanel != null) screenPanel.color = tooLongColor;
                if (statusText != null) statusText.text = tooLongMessage;

                UpdateLabelHighlight(3);
                SetInstructorMessage(tooLongInstruction);
                PlayClip(tooLongClip);
                PlayDoubleHaptics(tooLongAmplitude, tooLongPulseDuration, tooLongGap);

                if (instructorVoiceController != null)
                {
                    instructorVoiceController.PlayTooLongVoiceDelayed(instructorVoiceDelayAfterFeedback);
                }

                break;
        }
    }

    private void SetInstructorMessage(string message)
    {
        if (instructorMessageText != null)
        {
            instructorMessageText.text = message;
        }
    }

    private void UpdateLabelHighlight(int stage)
    {
        if (label15s != null) label15s.color = labelNormalColor;
        if (label30s != null) label30s.color = labelNormalColor;
        if (label45s != null) label45s.color = labelNormalColor;

        switch (stage)
        {
            case 1:
                if (label15s != null) label15s.color = labelSelectedColor;
                break;

            case 2:
                if (label30s != null) label30s.color = labelSelectedColor;
                break;

            case 3:
                if (label45s != null) label45s.color = labelSelectedColor;
                break;
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (uiAudioSource == null || clip == null) return;

        uiAudioSource.PlayOneShot(clip);
    }

    private void PlaySingleHaptics(float amplitude, float duration)
    {
        StopHapticsRoutineIfRunning();
        hapticRoutine = StartCoroutine(HapticPulseRoutine(amplitude, duration));
    }

    private void PlayDoubleHaptics(float amplitude, float pulseDuration, float gap)
    {
        StopHapticsRoutineIfRunning();
        hapticRoutine = StartCoroutine(DoubleHapticPulseRoutine(amplitude, pulseDuration, gap));
    }

    private void StopHapticsRoutineIfRunning()
    {
        if (hapticRoutine != null)
        {
            StopCoroutine(hapticRoutine);
            hapticRoutine = null;
        }
    }

    private IEnumerator HapticPulseRoutine(float amplitude, float duration)
    {
        SendHapticsToBoth(amplitude, duration);
        yield return new WaitForSeconds(duration);
        hapticRoutine = null;
    }

    private IEnumerator DoubleHapticPulseRoutine(float amplitude, float pulseDuration, float gap)
    {
        SendHapticsToBoth(amplitude, pulseDuration);
        yield return new WaitForSeconds(pulseDuration + gap);

        SendHapticsToBoth(amplitude, pulseDuration);
        yield return new WaitForSeconds(pulseDuration);

        hapticRoutine = null;
    }

    private void SendHapticsToBoth(float amplitude, float duration)
    {
        amplitude = Mathf.Clamp01(amplitude);
        duration = Mathf.Max(0f, duration);

        SendHapticToNode(XRNode.LeftHand, amplitude, duration);
        SendHapticToNode(XRNode.RightHand, amplitude, duration);
    }

    private void SendHapticToNode(XRNode node, float amplitude, float duration)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);

        if (!device.isValid) return;
        if (!device.TryGetHapticCapabilities(out HapticCapabilities capabilities)) return;
        if (!capabilities.supportsImpulse) return;

        device.SendHapticImpulse(0u, amplitude, duration);
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
}
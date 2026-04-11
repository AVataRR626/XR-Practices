using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DetectionStationController : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor scaleSocket;
    [SerializeField] private XRGrabInteractable reagentBeaker;

    [SerializeField] private Image screenPanel;
    [SerializeField] private TMP_Text statusText;

    [SerializeField] private Color idleColor = new Color(0.08f, 0.08f, 0.08f, 0.95f);
    [SerializeField] private Color detectedColor = new Color(0.12f, 0.55f, 0.18f, 0.95f);

    [SerializeField]
    private string detectedMessage =
        "Reagent detected. The reagent has been placed on the device.";

    private void Start()
    {
        Debug.Log("DetectionStationController started.");
        SetIdleState();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.f6Key.wasPressedThisFrame)
        {
            Debug.Log("F6 pressed -> detected state");
            SetDetectedState();
        }

        if (Keyboard.current.f7Key.wasPressedThisFrame)
        {
            Debug.Log("F7 pressed -> idle state");
            SetIdleState();
        }
    }

    private void OnEnable()
    {
        if (scaleSocket != null)
        {
            scaleSocket.selectEntered.AddListener(OnSocketSelectEntered);
            scaleSocket.selectExited.AddListener(OnSocketSelectExited);
        }
    }

    private void OnDisable()
    {
        if (scaleSocket != null)
        {
            scaleSocket.selectEntered.RemoveListener(OnSocketSelectEntered);
            scaleSocket.selectExited.RemoveListener(OnSocketSelectExited);
        }
    }

    private void OnSocketSelectEntered(SelectEnterEventArgs args)
    {
        if (args == null || args.interactableObject == null)
            return;

        Debug.Log("Socket entered by: " + args.interactableObject.transform.name);

        if (reagentBeaker != null && args.interactableObject.transform == reagentBeaker.transform)
        {
            SetDetectedState();
        }
    }

    private void OnSocketSelectExited(SelectExitEventArgs args)
    {
        if (args == null || args.interactableObject == null)
            return;

        Debug.Log("Socket exited by: " + args.interactableObject.transform.name);

        if (reagentBeaker != null && args.interactableObject.transform == reagentBeaker.transform)
        {
            SetIdleState();
        }
    }

    private void SetIdleState()
    {
        if (screenPanel != null)
            screenPanel.color = idleColor;

        if (statusText != null)
        {
            statusText.text = "";
            statusText.gameObject.SetActive(false);
        }

        Debug.Log("Idle state applied.");
    }

    private void SetDetectedState()
    {
        if (screenPanel != null)
            screenPanel.color = detectedColor;

        if (statusText != null)
        {
            statusText.gameObject.SetActive(true);
            statusText.text = detectedMessage;
        }

        Debug.Log("Detected state applied.");
    }
}
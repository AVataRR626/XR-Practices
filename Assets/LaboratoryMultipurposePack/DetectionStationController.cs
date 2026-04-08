using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DetectionStationController : MonoBehaviour
{
    [Header("Socket")]
    [SerializeField] private XRSocketInteractor scaleSocket;
    [SerializeField] private string reagentTag = "Reagent";

    [Header("Computer Screen")]
    [SerializeField] private Renderer screenFillRenderer;
    [SerializeField] private string colorPropertyName = "_BaseColor"; // URP常用；若无效改成"_Color"
    [SerializeField] private Color idleColor = new Color(0.12f, 0.12f, 0.12f, 1f);
    [SerializeField] private Color detectedColor = new Color(0.1f, 0.75f, 0.25f, 1f);

    [Header("UI")]
    [SerializeField] private Image statusPanel;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private string detectedMessage = "Reagent detected. The reagent has been placed on the device.";
    [SerializeField] private string idleMessage = "Device standby.";

    private Material screenMat;

    private void Awake()
    {
        if (screenFillRenderer != null)
        {
            screenMat = screenFillRenderer.material;
        }

        SetIdleState();
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
        if (args == null || args.interactableObject == null) return;

        Transform selected = args.interactableObject.transform;

        if (!selected.CompareTag(reagentTag)) return;

        SetDetectedState();
    }

    private void OnSocketSelectExited(SelectExitEventArgs args)
    {
        SetIdleState();
    }

    private void SetIdleState()
    {
        if (screenMat != null)
        {
            if (screenMat.HasProperty(colorPropertyName))
                screenMat.SetColor(colorPropertyName, idleColor);
        }

        if (statusPanel != null)
            statusPanel.color = new Color(0f, 0f, 0f, 0.65f);

        if (statusText != null)
            statusText.text = idleMessage;
    }

    private void SetDetectedState()
    {
        if (screenMat != null)
        {
            if (screenMat.HasProperty(colorPropertyName))
                screenMat.SetColor(colorPropertyName, detectedColor);
        }

        if (statusPanel != null)
            statusPanel.color = new Color(0.05f, 0.25f, 0.05f, 0.85f);

        if (statusText != null)
            statusText.text = detectedMessage;
    }
}
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class KnobBaseGlowPrompt : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private XRSocketInteractor deviceSocket;
    [SerializeField] private XRGrabInteractable reagentBeaker;
    [SerializeField] private XRSimpleInteractable timeSelectorInteractable;
    [SerializeField] private Renderer knobBaseRenderer;

    [Header("Materials")]
    [SerializeField] private Material idleMaterial;
    [SerializeField] private Material glowMaterial;

    private bool glowShown = false;

    private void Start()
    {
        ApplyIdle();
    }

    private void OnEnable()
    {
        if (deviceSocket != null)
        {
            deviceSocket.selectEntered.AddListener(OnSocketEntered);
        }

        if (timeSelectorInteractable != null)
        {
            timeSelectorInteractable.selectEntered.AddListener(OnKnobSelected);
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
            timeSelectorInteractable.selectEntered.RemoveListener(OnKnobSelected);
        }

        ApplyIdle();
        glowShown = false;
    }

    private void OnSocketEntered(SelectEnterEventArgs args)
    {
        if (args == null || args.interactableObject == null) return;
        if (reagentBeaker == null) return;
        if (args.interactableObject.transform != reagentBeaker.transform) return;

        ShowGlow();
    }

    private void OnKnobSelected(SelectEnterEventArgs args)
    {
        if (!glowShown) return;
        HideGlow();
    }

    private void ShowGlow()
    {
        if (knobBaseRenderer == null || glowMaterial == null) return;

        knobBaseRenderer.material = glowMaterial;
        glowShown = true;
    }

    private void HideGlow()
    {
        ApplyIdle();
        glowShown = false;
    }

    private void ApplyIdle()
    {
        if (knobBaseRenderer == null || idleMaterial == null) return;

        knobBaseRenderer.material = idleMaterial;
    }
}
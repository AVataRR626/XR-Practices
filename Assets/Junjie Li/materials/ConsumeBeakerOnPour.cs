using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ConsumeBeakerOnPour : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor socketInteractor;
    [SerializeField] private XRGrabInteractable reagentBeaker;
    [SerializeField] private float hideDelay = 0.08f;

    private bool hasConsumed = false;
    private Coroutine consumeCoroutine;

    private void OnEnable()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.AddListener(OnSocketEntered);
        }
    }

    private void OnDisable()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.RemoveListener(OnSocketEntered);
        }
    }

    private void OnSocketEntered(SelectEnterEventArgs args)
    {
        if (hasConsumed) return;
        if (args == null || args.interactableObject == null) return;
        if (reagentBeaker == null) return;
        if (args.interactableObject.transform != reagentBeaker.transform) return;

        if (consumeCoroutine != null)
        {
            StopCoroutine(consumeCoroutine);
        }

        consumeCoroutine = StartCoroutine(ConsumeRoutine());
    }

    private IEnumerator ConsumeRoutine()
    {
        yield return new WaitForSeconds(hideDelay);

        hasConsumed = true;

        Renderer[] renderers = reagentBeaker.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }

        Collider[] colliders = reagentBeaker.GetComponentsInChildren<Collider>(true);
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }

        Rigidbody rb = reagentBeaker.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }
}
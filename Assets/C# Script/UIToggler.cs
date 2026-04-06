using UnityEngine;
using UnityEngine.InputSystem;

public class UIToggler : MonoBehaviour
{
    [Header("")]
    public GameObject uiPanel;

    [Header("")]
    public InputAction toggleKey;

    [Header("")]
    public Transform headCamera;
    public float spawnDistance = 0.7f;
    public float heightOffset = -0.15f;

    private void OnEnable()
    {
        toggleKey.Enable();
        toggleKey.performed += ToggleContext;
    }

    private void OnDisable()
    {
        toggleKey.Disable();
        toggleKey.performed -= ToggleContext;
    }

    private void ToggleContext(InputAction.CallbackContext context)
    {
        TogglePanelLogic();
    }

    private void Update()
    {
  
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            TogglePanelLogic();
        }
    }

    private void TogglePanelLogic()
    {
        if (uiPanel != null && headCamera != null)
        {
            bool isOpening = !uiPanel.activeSelf;
            uiPanel.SetActive(isOpening);

            if (isOpening)
            {
                Vector3 targetPos = headCamera.position + headCamera.forward * spawnDistance;
                targetPos.y += heightOffset; 
                uiPanel.transform.position = targetPos;

                Vector3 lookDirection = uiPanel.transform.position - headCamera.position;
                uiPanel.transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }
}
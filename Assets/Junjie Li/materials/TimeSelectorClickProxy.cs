using UnityEngine;

public class TimeSelectorClickProxy : MonoBehaviour
{
    [SerializeField] private DetectionTimeSelectorController controller;

    private void OnMouseDown()
    {
        if (controller != null)
        {
            controller.AdvanceTimeSelection();
        }
    }
}
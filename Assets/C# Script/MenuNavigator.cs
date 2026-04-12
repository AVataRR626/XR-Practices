/*using UnityEngine;

public class MenuNavigator : MonoBehaviour
{
    [Header("mainpanel")]
    public GameObject mainMenuCanvas;

    [Header("secondpanel")]
    public GameObject backgroundAudioPanel;
    public GameObject alarmAudioPanel;
    public GameObject globalAudioPanel;

    //Click Background Audio
    public void OpenBackgroundPanel()
    {
        mainMenuCanvas.SetActive(false); // hide main
        if(backgroundAudioPanel != null) backgroundAudioPanel.SetActive(true); // open background audio
    }

    // Click Alarm Audio 
    public void OpenAlarmPanel()
    {
        mainMenuCanvas.SetActive(false);
        if(alarmAudioPanel != null) alarmAudioPanel.SetActive(true);
    }

    // Click All 
    public void OpenGlobalPanel()
    {
        mainMenuCanvas.SetActive(false);
        if(globalAudioPanel != null) globalAudioPanel.SetActive(true);
    }

    // Click Quit 
    public void QuitMenu()
    {
        mainMenuCanvas.SetActive(false); // Quit
    }
}
using UnityEngine;

public class MenuNavigator : MonoBehaviour
{
    [Header("main")]
    public GameObject mainMenuCanvas;

    [Header("targetboard")]
    public GameObject backgroundAudioPanel;
    public GameObject alarmAudioPanel;
    public GameObject globalAudioPanel;

    [Header("spaceposition")]
    public Transform headCamera;       // main camera
    public float spawnDistance = 0.7f; // distance
    public float heightOffset = -0.15f;// high

    public void OpenBackgroundPanel()
    {
        mainMenuCanvas.SetActive(false);
        SummonPanelToPlayer(backgroundAudioPanel);
    }

    public void OpenAlarmPanel()
    {
        mainMenuCanvas.SetActive(false);
        SummonPanelToPlayer(alarmAudioPanel);
    }

    public void OpenGlobalPanel()
    {
        mainMenuCanvas.SetActive(false);
        SummonPanelToPlayer(globalAudioPanel);
    }

    public void QuitMenu()
    {
        mainMenuCanvas.SetActive(false);
    }

    
    private void SummonPanelToPlayer(GameObject panel)
    {
        if (panel != null && headCamera != null)
        {
            panel.SetActive(true);

            // 1. position=camera position + -> * distance
            Vector3 targetPos = headCamera.position + headCamera.forward * spawnDistance;
            targetPos.y += heightOffset; 
            panel.transform.position = targetPos;

            // 2. turn to face
            Vector3 lookDirection = panel.transform.position - headCamera.position;
            panel.transform.rotation = Quaternion.LookRotation(lookDirection);
        }
        else if (panel != null)
        {
            // if error
            panel.SetActive(true);
            Debug.LogWarning("MenuNavigator");
        }
    }
}
using UnityEngine;

// 3 Audio settings
[System.Serializable]
public class AudioGroupData
{
    [Header("3 Audio source")]
    public AudioSource[] sources = new AudioSource[3];
    [Header("3 Audio source name")]
    public string[] trackNames = new string[3] { "Audio 1", "Audio 2", "Audio 3" };
    [Header("3 Audio source AudioMixer number")]
    public string[] lowParams = new string[3];
    public string[] midParams = new string[3];
    public string[] highParams = new string[3];
}

public class MenuNavigator : MonoBehaviour
{
    [Header("=== Call Broad ===")]
    public GameObject mainMenuCanvas;
    public GameObject eqPanelCanvas;
    public DynamicEQController eqController; // EQ controller

    [Header("=== Position ===")]
    public Transform headCamera;
    public float spawnDistance = 0.7f;
    public float heightOffset = -0.15f;

    [Header("=== Audio DATA ===")]
    public AudioGroupData backgroundGroup;
    public AudioGroupData alarmGroup;

    public void OpenBackgroundPanel()
    {
        mainMenuCanvas.SetActive(false);
        SummonPanelToPlayer(eqPanelCanvas);
        // sent Background Audio data to EQ broad
        if(eqController != null)
            eqController.SetupPanelGroup(backgroundGroup.sources, backgroundGroup.trackNames, backgroundGroup.lowParams, backgroundGroup.midParams, backgroundGroup.highParams);
    }

    public void OpenAlarmPanel()
    {
        mainMenuCanvas.SetActive(false);
        SummonPanelToPlayer(eqPanelCanvas);
        // sent Alarm Audio data to EQ broad
        if(eqController != null)
            eqController.SetupPanelGroup(alarmGroup.sources, alarmGroup.trackNames, alarmGroup.lowParams, alarmGroup.midParams, alarmGroup.highParams);
    }

    public void QuitMenu() { mainMenuCanvas.SetActive(false); }

    private void SummonPanelToPlayer(GameObject panel)
    {
        if (panel != null && headCamera != null)
        {
            panel.SetActive(true);
            Vector3 targetPos = headCamera.position + headCamera.forward * spawnDistance;
            targetPos.y += heightOffset; 
            panel.transform.position = targetPos;
            panel.transform.rotation = Quaternion.LookRotation(panel.transform.position - headCamera.position);
        }
    }
}*/

using UnityEngine;

// 3 Audio settings
[System.Serializable]
public class AudioGroupData
{
    [Header("3 Audio source")]
    public AudioSource[] sources = new AudioSource[3];
    [Header("3 Audio source name")]
    public string[] trackNames = new string[3] { "Audio 1", "Audio 2", "Audio 3" };
    [Header("3 Audio source AudioMixer number")]
    public string[] lowParams = new string[3];
    public string[] midParams = new string[3];
    public string[] highParams = new string[3];
}

public class MenuNavigator : MonoBehaviour
{
    [Header("=== Call Broad ===")]
    public GameObject mainMenuCanvas;
    public GameObject eqPanelCanvas;
    public GameObject globalAudioPanel; // ADD Global/ALL 
    public DynamicEQController eqController; // EQ controller

    [Header("=== Position ===")]
    public Transform headCamera;
    public float spawnDistance = 0.7f;
    public float heightOffset = -0.15f;

    [Header("=== Audio DATA ===")]
    public AudioGroupData backgroundGroup;
    public AudioGroupData alarmGroup;

    public void OpenBackgroundPanel()
    {
        mainMenuCanvas.SetActive(false);
        SummonPanelToPlayer(eqPanelCanvas);
        // sent Background Audio data to EQ broad
        if(eqController != null)
            eqController.SetupPanelGroup(backgroundGroup.sources, backgroundGroup.trackNames, backgroundGroup.lowParams, backgroundGroup.midParams, backgroundGroup.highParams);
    }

    public void OpenAlarmPanel()
    {
        mainMenuCanvas.SetActive(false);
        SummonPanelToPlayer(eqPanelCanvas);
        // sent Alarm Audio data to EQ broad
        if(eqController != null)
            eqController.SetupPanelGroup(alarmGroup.sources, alarmGroup.trackNames, alarmGroup.lowParams, alarmGroup.midParams, alarmGroup.highParams);
    }

    // ADD open Global (ALL) 
    public void OpenGlobalPanel()
    {
        mainMenuCanvas.SetActive(false);
        SummonPanelToPlayer(globalAudioPanel);
    }

    public void QuitMenu() { mainMenuCanvas.SetActive(false); }

    private void SummonPanelToPlayer(GameObject panel)
    {
        if (panel != null && headCamera != null)
        {
            panel.SetActive(true);
            Vector3 targetPos = headCamera.position + headCamera.forward * spawnDistance;
            targetPos.y += heightOffset; 
            panel.transform.position = targetPos;
            panel.transform.rotation = Quaternion.LookRotation(panel.transform.position - headCamera.position);
        }
    }
}
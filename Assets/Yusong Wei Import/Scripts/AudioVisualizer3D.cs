/*using UnityEngine;

public class AudioVisualizer3D : MonoBehaviour
{
    [Header("=== volum (dB) ===")]
    public Transform volumeBar; // volum 3D cube
    public float volumeMultiplier = 15f;
    public float maxVolumeHeight = 2f; // limit MAX volum

    [Header("===  (Hz) Group ===")]
    public GameObject spectrumBarPrefab; // perfab 3D  (BarPivot)
    [Tooltip("cube，32 or 64")]
    public int numberOfBars = 64;
    [Tooltip("space between cube")]
    public float spacing = 0.3f; 
    public float barHeightMultiplier = 50f;
    public float maxBarHeight = 2.5f; // limit MAX height of CUBE

    // data save
    private float[] audioSamples = new float[256];
    private float[] spectrumData = new float[64];
    private Transform[] generatedBars;

    private void Start()
    {
        // create 3D cube
        if (spectrumBarPrefab != null)
        {
            generatedBars = new Transform[numberOfBars];
            for (int i = 0; i < numberOfBars; i++)
            {
                // 
                GameObject newBar = Instantiate(spectrumBarPrefab, transform);
                // -> X line
                newBar.transform.localPosition = new Vector3(i * spacing, 0, 0);
                generatedBars[i] = newBar.transform;
            }
        }
    }

    private void Update()
    {
        // ================= 1. volum (dB) =================
        if (volumeBar != null)
        {
            AudioListener.GetOutputData(audioSamples, 0);
            float sum = 0f;
            for (int i = 0; i < 256; i++) sum += audioSamples[i] * audioSamples[i];
            float rmsValue = Mathf.Sqrt(sum / 256f);

            // use maxVolumeHeight limit height-> 20f
            float targetHeight = Mathf.Clamp(rmsValue * volumeMultiplier, 0.1f, maxVolumeHeight); 
            Vector3 newScale = volumeBar.localScale;
            newScale.y = Mathf.Lerp(newScale.y, targetHeight, Time.deltaTime * 15f);
            volumeBar.localScale = newScale;
        }

        // ================= 2. (Hz) =================
        if (generatedBars != null)
        {
            AudioListener.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

            for (int i = 0; i < numberOfBars; i++)
            {
                if (generatedBars[i] != null)
                {
                    // (i + 1) UIdesign for clear and beautiful
                    float targetHeight = spectrumData[i] * barHeightMultiplier * (i + 1);
                    // use maxBarHeight limit height-> 50f
                    targetHeight = Mathf.Clamp(targetHeight, 0.03f, maxBarHeight); 

                    Vector3 newScale = generatedBars[i].localScale;
                    newScale.y = Mathf.Lerp(newScale.y, targetHeight, Time.deltaTime * 15f);
                    generatedBars[i].localScale = newScale;
                }
            }
        }
    }
}*/

using UnityEngine;

public class AudioVisualizer3D : MonoBehaviour
{
    [Header("=== Volume (dB) ===")]
    [Tooltip("Use Volume_MasterBar gameobject")]
    public Transform volumeBar; // 3D cube that will become volume visualiser
    public float volumeMultiplier = 15f;
    public float maxVolumeHeight = 2f; // limit MAX volum

    [Header("===  Frequency Group (Hz) ===")]
    [Tooltip("Use BarPivot prefab")]
    public GameObject spectrumBarPrefab; // 3D cube (BarPivot) that will become frequency visualiser
    [Tooltip("Recommend using 64")]
    public int numberOfBars = 64;
    [Tooltip("Space between cube")]
    public float spacing = 0.3f; 
    public float barHeightMultiplier = 50f;
    public float maxBarHeight = 2.5f; // limit MAX height of CUBE

    // [NEW] === Color Gradient  ===
    [Header("=== Color Gradient ===")]
    [ColorUsage(true, true)] //  OPEN HDR 
    public Color lowColor = Color.blue; // the lowest color
    [ColorUsage(true, true)]
    public Color highColor = Color.red; // the highest color

    // data save
    private float[] audioSamples = new float[1024];
    private float[] spectrumData = new float[256];
    private Transform[] generatedBars;

    // [NEW] change color
    private Renderer[] barRenderers; 
    private MaterialPropertyBlock propBlock; 

    private void Start()
    {
        // [NEW] reset
        propBlock = new MaterialPropertyBlock();

        // create 3D cube
        if (spectrumBarPrefab != null)
        {
            generatedBars = new Transform[numberOfBars];
            barRenderers = new Renderer[numberOfBars]; // 

            for (int i = 0; i < numberOfBars; i++)
            {
                // 
                GameObject newBar = Instantiate(spectrumBarPrefab, transform);

               

                // -> X line
                newBar.transform.localPosition = new Vector3(i * spacing, 0, 0);
                generatedBars[i] = newBar.transform;

                // [NEW] catch each cube and add (MeshRenderer)
                barRenderers[i] = newBar.GetComponentInChildren<Renderer>();
            }
        }
    }

    private void Update()
    {
        // ================= 1. volum (dB) =================
        if (volumeBar != null)
        {
            AudioListener.GetOutputData(audioSamples, 0);
            float sum = 0f;
            for (int i = 0; i < 256; i++) sum += audioSamples[i] * audioSamples[i];
            float rmsValue = Mathf.Sqrt(sum / 256f);

            // use maxVolumeHeight limit height-> 20f
            float targetHeight = Mathf.Clamp(rmsValue * volumeMultiplier, 0.1f, maxVolumeHeight); 
            Vector3 newScale = volumeBar.localScale;
            newScale.y = Mathf.Lerp(newScale.y, targetHeight, Time.deltaTime * 15f);
            volumeBar.localScale = newScale;
        }

        // ================= 2. (Hz) & Gradient Color =================
        if (generatedBars != null)
        {
            AudioListener.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

            for (int i = 0; i < numberOfBars; i++)
            {
                if (generatedBars[i] != null)
                {
                    // (i + 1) UIdesign for clear and beautiful
                    float targetHeight = spectrumData[i] * barHeightMultiplier * (i + 1);
                    // use maxBarHeight limit height-> 50f
                    targetHeight = Mathf.Clamp(targetHeight, 0.005f, maxBarHeight); 

                    Vector3 newScale = generatedBars[i].localScale;
                    newScale.y = Mathf.Lerp(newScale.y, targetHeight, Time.deltaTime * 15f);
                    generatedBars[i].localScale = newScale;

                    Debug.Log(targetHeight);

                    // [NEW] === color change ===
                    if (barRenderers[i] != null)
                    {
                        // Percentage of the highest point in the current accounting period (0.0 - 1.0)
                        float heightRatio = Mathf.Clamp01(newScale.y / maxBarHeight);
                        
                        // Mix blue and red according to percentage
                        Color currentColor = Color.Lerp(lowColor, highColor, heightRatio);

                        // add shine
                        barRenderers[i].GetPropertyBlock(propBlock);
                        propBlock.SetColor("_EmissionColor", currentColor);
                        barRenderers[i].SetPropertyBlock(propBlock);
                    }
                }
                
            }
        }
    }
}


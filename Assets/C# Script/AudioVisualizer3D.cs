using UnityEngine;

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
                    targetHeight = Mathf.Clamp(targetHeight, 0.1f, maxBarHeight); 

                    Vector3 newScale = generatedBars[i].localScale;
                    newScale.y = Mathf.Lerp(newScale.y, targetHeight, Time.deltaTime * 15f);
                    generatedBars[i].localScale = newScale;
                }
            }
        }
    }
}
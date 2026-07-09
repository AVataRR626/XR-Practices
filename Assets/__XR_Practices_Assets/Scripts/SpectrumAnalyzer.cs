using UnityEngine;

public class SpectrumAnalyzer : MonoBehaviour
{
    public AudioSource audioSource;
   
    //Must be a power of 2: 128, 256, 512, 1024, etc. 1024 is most commonn for detailed resolution
    public int sampleSize = 1024;
    
    public float[] spectrumData;
    
    void Start()
    {
        spectrumData = new float[sampleSize];
    }

    // Update is called once per frame
    void Update()
    {
        //populate the array with current spectrum data
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        //accessing the amplitude of a specific frequency index
        int targetIndex = 10;
        float amplitude = spectrumData[targetIndex];

        Debug.Log($"Amplitude at index {targetIndex}: {amplitude}");


    }
    
}

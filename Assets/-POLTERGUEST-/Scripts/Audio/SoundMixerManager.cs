using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("VolumenGlobal", Mathf.Log10(level) * 20f);
    }

    public void SetSFXVolume(float level)
    {
        audioMixer.SetFloat("VolumenSFX", Mathf.Log10(level) * 20f);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("VolumenMusica", Mathf.Log10(level) * 20f);
    }

    public void SetAmbientVolume(float level)
    {
        audioMixer.SetFloat("VolumenAmbiente", Mathf.Log10(level) * 20f);
    }
}
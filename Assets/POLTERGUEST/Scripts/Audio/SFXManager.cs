using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [SerializeField] private AudioSource SFXObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlaySFX(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // Crear gameObject
        AudioSource audioSource = Instantiate(SFXObject, spawnTransform.position, Quaternion.identity);
    
        // Asignar el audioClip
        audioSource.clip = audioClip;

        //Asignar el volumen
        audioSource.volume = volume;

        //Reproducir el sonido
        audioSource.Play(); 

        //Obtener longitud del clip de audio
        float clipLength = audioSource.clip.length;

        //Destruir el gameObject después de reproducirse
        Destroy(audioSource.gameObject, clipLength);
    }

    public AudioSource PlayLoopingSFX(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(
            SFXObject,
            spawnTransform.position,
            Quaternion.identity
        );
    
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = true;
    
        audioSource.Play();
    
        return audioSource;
    }
}

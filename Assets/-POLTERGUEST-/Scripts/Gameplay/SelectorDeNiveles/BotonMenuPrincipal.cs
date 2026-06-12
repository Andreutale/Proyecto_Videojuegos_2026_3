using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonMenuPrincipal : MonoBehaviour
{
    [SerializeField] private AudioClip sonidoClic;

    public void AlHacerClic()
    {
        if (sonidoClic != null)
            SFXManager.Instance.PlaySFX(sonidoClic, transform, 1f);

        SceneManager.LoadScene(0);
    }
}
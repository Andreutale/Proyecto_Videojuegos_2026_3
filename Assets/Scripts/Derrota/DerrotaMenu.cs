using UnityEngine;
using UnityEngine.SceneManagement;

public class DerrotaMenu : MonoBehaviour
{
    public AudioSource sonidoDerrota;

    [Header("Sonido Botones")]
    [SerializeField] private AudioClip sonidoReiniciar;
    [SerializeField] private AudioClip sonidoMenuPrincipal;

    void OnEnable()
    {
        if (sonidoDerrota != null)
        {
            sonidoDerrota.Play();
        }
    }

    public void ReiniciarNivel()
    {
        if (sonidoReiniciar != null)
            SFXManager.Instance.PlaySFX(sonidoReiniciar, transform, 1f);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MenuPrincipal()
    {
        if (sonidoMenuPrincipal != null)
            SFXManager.Instance.PlaySFX(sonidoMenuPrincipal, transform, 1f);

        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuInicial");
    }
}
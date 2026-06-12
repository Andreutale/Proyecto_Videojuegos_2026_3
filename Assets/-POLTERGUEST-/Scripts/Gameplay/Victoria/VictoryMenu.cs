using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryMenu : MonoBehaviour
{
    public string siguienteNivel = "Nivel_Prototipo";

    [Header("Sonido")]
    [SerializeField] private AudioClip sonidoSiguienteNivel;
    [SerializeField] private AudioClip sonidoReiniciar;
    [SerializeField] private AudioClip sonidoMenuPrincipal;

    public void SiguienteNivel()
    {
        if (sonidoSiguienteNivel != null)
            SFXManager.Instance.PlaySFX(sonidoSiguienteNivel, transform, 1f);

        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelector");
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
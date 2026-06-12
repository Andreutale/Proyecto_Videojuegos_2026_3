using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitialMenu : MonoBehaviour
{
    [Header("Configuración del Fundido")]
    public Image panelFundido;
    public float velocidadFundido = 1.5f;

    [Header("Sonido")]
    [SerializeField] private AudioClip sonidoJugar;
    [SerializeField] private AudioClip sonidoSalir;

    void Start()
    {
        if (panelFundido != null)
        {
            Color colorInicial = panelFundido.color;
            colorInicial.a = 0f;
            panelFundido.color = colorInicial;
            panelFundido.gameObject.SetActive(false);
        }
    }

    public void Jugar()
    {
        if (sonidoJugar != null)
            SFXManager.Instance.PlaySFX(sonidoJugar, transform, 1f);

        StartCoroutine(FundidoYCarga());
    }

    private IEnumerator FundidoYCarga()
    {
        if (panelFundido != null)
        {
            panelFundido.gameObject.SetActive(true);
            float alpha = 0f;
            Color colorActual = panelFundido.color;

            while (alpha < 1f)
            {
                alpha += Time.deltaTime * velocidadFundido;
                colorActual.a = alpha;
                panelFundido.color = colorActual;
                yield return null;
            }
        }

        SceneManager.LoadScene("LevelSelector");
    }

    public void Salir()
    {
        if (sonidoSalir != null)
            SFXManager.Instance.PlaySFX(sonidoSalir, transform, 1f);

        Debug.Log("Salir...");
        Application.Quit();
    }
}
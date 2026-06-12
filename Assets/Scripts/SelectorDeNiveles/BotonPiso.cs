using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BotonPiso : MonoBehaviour
{
    public string nombreNivel;
    public int indicePiso;
    public bool estaBloqueado;

    public GameObject candado;
    public GameObject estrellas;

    public Image estrella1;
    public Image estrella2;
    public Image estrella3;

    public AnimacionPuerta animacionPuerta;

    [SerializeField] private AudioClip sonidoClic;
    [SerializeField] private GameObject panelMensaje;

    private Color colorEncendida = Color.white;
    private Color colorApagada = new Color(0.3f, 0.3f, 0.3f, 0.5f);

    void Awake()
    {
        if (indicePiso == 4)
            estaBloqueado = PlayerPrefs.GetInt("Nivel_1_Completado", 0) == 0;

        if (indicePiso == 1)
        {
            int estrellasNivel1 = PlayerPrefs.GetInt("Nivel_1_Estrellas", 0);
            int estrellasNivel2 = PlayerPrefs.GetInt("Nuevo_Modelo_Habitacion_Estrellas", 0);
            int totalEstrellas = estrellasNivel1 + estrellasNivel2;
            estaBloqueado = totalEstrellas < 5;
        }

        if (estaBloqueado)
        {
            candado.SetActive(true);
            estrellas.SetActive(false);
        }
        else
        {
            AnimacionDesbloqueo anim = GetComponent<AnimacionDesbloqueo>();
            if (anim == null)
                candado.SetActive(false);

            int mejorEstrellas = PlayerPrefs.GetInt(nombreNivel + "_Estrellas", 0);

            if (mejorEstrellas == 0)
            {
                estrellas.SetActive(false);
            }
            else
            {
                estrellas.SetActive(true);
                estrella1.color = mejorEstrellas >= 1 ? colorEncendida : colorApagada;
                estrella2.color = mejorEstrellas >= 2 ? colorEncendida : colorApagada;
                estrella3.color = mejorEstrellas >= 3 ? colorEncendida : colorApagada;
            }
        }
    }

    public void AlHacerClic()
    {
        if (estaBloqueado)
        {
            if (panelMensaje != null)
                StartCoroutine(MostrarMensaje());
        }
        else
        {
            if (sonidoClic != null)
                SFXManager.Instance.PlaySFX(sonidoClic, transform, 1f);

            if (animacionPuerta != null)
                animacionPuerta.EntrarPuerta();
            else
                SceneManager.LoadScene(indicePiso);
        }
    }

    private IEnumerator MostrarMensaje()
    {
        panelMensaje.SetActive(true);
        yield return new WaitForSeconds(5f);
        panelMensaje.SetActive(false);
    }
}
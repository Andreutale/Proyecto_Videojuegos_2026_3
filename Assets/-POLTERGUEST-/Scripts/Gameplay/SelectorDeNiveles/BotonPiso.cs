using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    private Color colorEncendida = Color.white;
    private Color colorApagada = new Color(0.3f, 0.3f, 0.3f, 0.5f);

    void Awake()
    {
        if (indicePiso == 4)
            estaBloqueado = PlayerPrefs.GetInt("Nivel_1_Completado", 0) == 0;

        if (indicePiso == 1)
            estaBloqueado = PlayerPrefs.GetInt("Nuevo_Modelo_Habitacion_Completado", 0) == 0;

        if (estaBloqueado)
        {
            candado.SetActive(true);
            estrellas.SetActive(false);
            Button btn = GetComponentInChildren<Button>();
            if (btn != null) btn.interactable = false;
        }
        else
        {
            AnimacionDesbloqueo anim = GetComponent<AnimacionDesbloqueo>();
            if (anim == null)
                candado.SetActive(false);

            Button btn = GetComponentInChildren<Button>();
            if (btn != null) btn.interactable = true;

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
        if (!estaBloqueado)
        {
            if (sonidoClic != null)
                SFXManager.Instance.PlaySFX(sonidoClic, transform, 1f);

            if (animacionPuerta != null)
                animacionPuerta.EntrarPuerta();
            else
                SceneManager.LoadScene(indicePiso);
        }
    }
}
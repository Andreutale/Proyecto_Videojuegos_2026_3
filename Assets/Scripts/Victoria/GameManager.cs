using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Llaves")]
    public int llavesRecogidas = 0;
    public int llavesTotales = 5;

    [Header("UI Victoria")]
    public GameObject panelVictoria;
    public TMP_Text textoLlavesVictoria;

    [Header("UI Derrota")]
    public GameObject panelDerrota;
    public TMP_Text textoLlavesDerrota;

    [Header("Pause")]
    public GameObject pauseButton;

    [Header("Estrellas")]
    public GestorEstrellas gestorEstrellas;
    public int cantidadEstrellas = 3;

    private bool nivelTerminado = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        llavesTotales = GameObject.FindGameObjectsWithTag("Llave").Length;
        llavesRecogidas = 0;

        if (panelDerrota != null)
            panelDerrota.SetActive(false);

        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        ActualizarTextoLlaves();
    }

    public void RecogerLlave()
    {
        llavesRecogidas++;

        if (llavesRecogidas > llavesTotales)
            llavesRecogidas = llavesTotales;

        ActualizarTextoLlaves();
    }

    public void FinalizarNivel()
    {
        if (nivelTerminado) return;
        nivelTerminado = true;

        if (pauseButton != null)
            pauseButton.SetActive(false);

        // Puan hesapla
        if (TemporizadorGlobal.Instance != null && GestorPuntuacion.Instance != null)
        {
            GestorPuntuacion.Instance.CalcularPuntuacionPorTiempo(TemporizadorGlobal.Instance.tiempoTranscurrido);

            int puntos = GestorPuntuacion.Instance.PuntuacionFinal;
            if (puntos >= 2000) cantidadEstrellas = 3;
            else if (puntos >= 1500) cantidadEstrellas = 2;
            else if (puntos >= 1000) cantidadEstrellas = 1;
            else cantidadEstrellas = 0;
        }

        if (panelVictoria != null)
            panelVictoria.SetActive(true);

        if (gestorEstrellas != null)
            gestorEstrellas.MostrarEstrellas(cantidadEstrellas);

        ActualizarTextoLlaves();

        string nombreNivel = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetInt(nombreNivel + "_Completado", 1);
        PlayerPrefs.Save();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void FinalizarDerrota()
    {
        if (nivelTerminado) return;
        nivelTerminado = true;

        if (pauseButton != null)
            pauseButton.SetActive(false);

        if (panelDerrota != null)
            panelDerrota.SetActive(true);

        ActualizarTextoLlaves();

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ActualizarTextoLlaves()
    {
        string texto = "Llaves: " + llavesRecogidas + "/" + llavesTotales;

        if (textoLlavesVictoria != null)
            textoLlavesVictoria.text = texto;

        if (textoLlavesDerrota != null)
            textoLlavesDerrota.text = texto;
    }
}
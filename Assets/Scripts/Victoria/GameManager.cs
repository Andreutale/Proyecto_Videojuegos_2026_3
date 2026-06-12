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
    public int cantidadEstrellas = 1;

    [Header("Estrellas - Tiempos límite (segundos)")]
    public float tiempoPara3Estrellas = 60f;
    public float tiempoPara2Estrellas = 120f;

    [Header("Puntuación")]
    public int puntuacionBase = 3000;
    public int penalizacionPorSegundo = 30;
    public int PuntuacionFinal { get; private set; }

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

        float tiempo = 0f;
        if (TemporizadorGlobal.Instance != null)
        {
            tiempo = TemporizadorGlobal.Instance.tiempoTranscurrido;
            int segundos = Mathf.FloorToInt(tiempo);
            PuntuacionFinal = puntuacionBase - (segundos * penalizacionPorSegundo);
            if (PuntuacionFinal < 0) PuntuacionFinal = 0;
        }

        // Calcular estrellas según el tiempo (mínimo 1 estrella)
        if (tiempo <= tiempoPara3Estrellas) cantidadEstrellas = 3;
        else if (tiempo <= tiempoPara2Estrellas) cantidadEstrellas = 2;
        else cantidadEstrellas = 1;

        if (panelVictoria != null)
            panelVictoria.SetActive(true);

        if (gestorEstrellas != null)
            gestorEstrellas.MostrarEstrellas(cantidadEstrellas);

        ActualizarTextoLlaves();

        string nombreNivel = SceneManager.GetActiveScene().name;

        int estrellasAnteriores = PlayerPrefs.GetInt(nombreNivel + "_Estrellas", 0);
        if (cantidadEstrellas > estrellasAnteriores)
            PlayerPrefs.SetInt(nombreNivel + "_Estrellas", cantidadEstrellas);

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
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ConfiguracionMenu : MonoBehaviour
{
    [Header("Instancia del Mixer")]
    public AudioMixer audioMixer;

    [Header("Sliders de Volumen")]
    public Slider sliderGlobal;
    public Slider sliderMusica;
    public Slider sliderEnemigos;
    public Slider sliderEfectos;

    [Header("Pantalla Completa")]
    public Toggle togglePantallaCompleta;
    public Image checkboxImagen; // Objeto 'Checkmark'
    public Sprite spriteActivado;  // Imagen casilla marcada (con tick)
    public Sprite spriteDesactivado; // Imagen casilla vacía

    [Header("Botón de Guardar (Imagen)")]
    public Image btnGuardar;

    [Header("Gestión de Menús para Salir")]
    public GameObject canvasAjustes; // Tu objeto 'CanvasAjustes'
    public GameObject pauseMenu;     // Tu objeto 'PauseMenu'

    // Variables temporales (las que se mueven en tiempo real con la UI)
    private bool estadoPantallaTemporal;
    private float volGlobalTemporal;
    private float volMusicaTemporal;
    private float volEnemigosTemporal;
    private float volEfectosTemporal;

    // Variables de respaldo (guardan el estado exacto al abrir para el botón Cancelar)
    private bool pantallaAlAbrir;
    private float volGlobalAlAbrir;
    private float volMusicaAlAbrir;
    private float volEnemigosAlAbrir;
    private float volEfectosAlAbrir;

    void Start()
    {
        // 1. Cargar la configuración real del perfil nada más iniciar el juego
        CargarConfiguracionInicialDelPerfil();

        // 2. Escuchar los cambios temporales que haga el usuario en los Sliders
        sliderGlobal.onValueChanged.AddListener(val => volGlobalTemporal = val);
        sliderMusica.onValueChanged.AddListener(val => volMusicaTemporal = val);
        sliderEnemigos.onValueChanged.AddListener(val => volEnemigosTemporal = val);
        sliderEfectos.onValueChanged.AddListener(val => volEfectosTemporal = val);

        if (togglePantallaCompleta != null)
        {
            togglePantallaCompleta.onValueChanged.AddListener(SeleccionarPantallaCompletaTemporal);
        }

        // 3. Configurar el click de la imagen de guardar automáticamente por código
        ConfigurarBotonGuardar();
    }

    private void CargarConfiguracionInicialDelPerfil()
    {
        volGlobalTemporal = PlayerPrefs.GetFloat("VolMaster", 1f);
        volMusicaTemporal = PlayerPrefs.GetFloat("VolMusica", 1f);
        volEnemigosTemporal = PlayerPrefs.GetFloat("VolEnemigos", 1f);
        volEfectosTemporal = PlayerPrefs.GetFloat("VolEfectos", 1f);
        estadoPantallaTemporal = (PlayerPrefs.GetInt("PantallaCompleta", 1) == 1);

        // Sincronizar UI física
        sliderGlobal.value = volGlobalTemporal;
        sliderMusica.value = volMusicaTemporal;
        sliderEnemigos.value = volEnemigosTemporal;
        sliderEfectos.value = volEfectosTemporal;

        if (togglePantallaCompleta != null)
        {
            togglePantallaCompleta.onValueChanged.RemoveListener(SeleccionarPantallaCompletaTemporal);
            togglePantallaCompleta.isOn = estadoPantallaTemporal;
            ActualizarVisualCheckbox(estadoPantallaTemporal);
            togglePantallaCompleta.onValueChanged.AddListener(SeleccionarPantallaCompletaTemporal);
        }

        // Aplicar volúmenes logarítmicos reales al AudioMixer
        SetMixerVolumeReal("VolMaster", volGlobalTemporal);
        SetMixerVolumeReal("VolMusica", volMusicaTemporal);
        SetMixerVolumeReal("VolEnemigos", volEnemigosTemporal);
        SetMixerVolumeReal("VolEfectos", volEfectosTemporal);
    }

    // Cada vez que se enciende el panel de ajustes, tomamos una "foto" de los valores
    void OnEnable()
    {
        volGlobalAlAbrir = sliderGlobal.value;
        volMusicaAlAbrir = sliderMusica.value;
        volEnemigosAlAbrir = sliderEnemigos.value;
        volEfectosAlAbrir = sliderEfectos.value;
        if (togglePantallaCompleta != null) pantallaAlAbrir = togglePantallaCompleta.isOn;

        // Igualamos los temporales para empezar desde aquí
        volGlobalTemporal = volGlobalAlAbrir;
        volMusicaTemporal = volMusicaAlAbrir;
        volEnemigosTemporal = volEnemigosAlAbrir;
        volEfectosTemporal = volEfectosAlAbrir;
        estadoPantallaTemporal = pantallaAlAbrir;
    }

    public void SeleccionarPantallaCompletaTemporal(bool esCompleta)
    {
        estadoPantallaTemporal = esCompleta;
        ActualizarVisualCheckbox(esCompleta);
    }

    // --- ACCIÓN DEL BOTÓN GUARDAR ---
    public void GuardarConfiguracion()
    {
        Debug.Log("Guardando configuración y regresando al juego...");

        // 1. Guardar y aplicar de forma definitiva en el perfil del jugador
        Screen.fullScreen = estadoPantallaTemporal;
        PlayerPrefs.SetInt("PantallaCompleta", estadoPantallaTemporal ? 1 : 0);

        AplicarYGuardarVolumen("VolMaster", volGlobalTemporal);
        AplicarYGuardarVolumen("VolMusica", volMusicaTemporal);
        AplicarYGuardarVolumen("VolEnemigos", volEnemigosTemporal);
        AplicarYGuardarVolumen("VolEfectos", volEfectosTemporal);
        PlayerPrefs.Save();

        // 2. Volver al juego de forma directa a través del PauseManager para que no desaparezca el botón de pausa
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
        {
            pauseManager.ResumeGame();
        }
        else
        {
            // Cierre de emergencia si no se encuentra el script
            if (canvasAjustes != null) canvasAjustes.SetActive(false);
        }
    }

    // --- ACCIÓN DEL BOTÓN CANCELAR ---
    public void CancelarCambios()
    {
        Debug.Log("Cancelando cambios. Restaurando valores previos y volviendo al menú de pausa...");

        // Devolvemos los sliders a su posición de la "foto" inicial
        sliderGlobal.value = volGlobalAlAbrir;
        sliderMusica.value = volMusicaAlAbrir;
        sliderEnemigos.value = volEnemigosAlAbrir;
        sliderEfectos.value = volEfectosAlAbrir;

        if (togglePantallaCompleta != null)
        {
            togglePantallaCompleta.onValueChanged.RemoveListener(SeleccionarPantallaCompletaTemporal);
            togglePantallaCompleta.isOn = pantallaAlAbrir;
            ActualizarVisualCheckbox(pantallaAlAbrir);
            togglePantallaCompleta.onValueChanged.AddListener(SeleccionarPantallaCompletaTemporal);
        }

        // Volvemos al menú de pausa normal
        if (canvasAjustes != null) canvasAjustes.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(true);
    }

    private void AplicarYGuardarVolumen(string nombreParametro, float valorSlider)
    {
        SetMixerVolumeReal(nombreParametro, valorSlider);
        PlayerPrefs.SetFloat(nombreParametro, valorSlider);
    }

    private void SetMixerVolumeReal(string nombreParametro, float valorSlider)
    {
        if (valorSlider <= 0.001f) audioMixer.SetFloat(nombreParametro, -80f);
        else audioMixer.SetFloat(nombreParametro, Mathf.Log10(valorSlider) * 20);
    }

    private void ActualizarVisualCheckbox(bool esCompleta)
    {
        if (checkboxImagen != null && spriteActivado != null && spriteDesactivado != null)
        {
            checkboxImagen.sprite = esCompleta ? spriteActivado : spriteDesactivado;
        }
    }

    private void ConfigurarBotonGuardar()
    {
        if (btnGuardar != null)
        {
            btnGuardar.raycastTarget = true;
            UnityEngine.EventSystems.EventTrigger trigger = btnGuardar.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (trigger == null) trigger = btnGuardar.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

            trigger.triggers.Clear();
            UnityEngine.EventSystems.EventTrigger.Entry entry = new UnityEngine.EventSystems.EventTrigger.Entry();
            entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { GuardarConfiguracion(); });
            trigger.triggers.Add(entry);
        }
    }
}
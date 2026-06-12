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
    public Image checkboxImagen;
    public Sprite spriteActivado;
    public Sprite spriteDesactivado;

    [Header("Botón de Guardar (Imagen)")]
    public Image btnGuardar;

    [Header("Gestión de Menús para Salir")]
    public GameObject canvasAjustes; // Arrastra aquí tu 'CanvasAjustes'
    public GameObject pauseMenu;     // Arrastra aquí tu 'PauseMenu'

    // Variables temporales (las que se mueven con los sliders)
    private bool estadoPantallaTemporal;
    private float volGlobalTemporal;
    private float volMusicaTemporal;
    private float volEnemigosTemporal;
    private float volEfectosTemporal;

    // Variables para guardar el estado exacto al abrir el menú (para el botón Cancelar)
    private bool pantallaAlAbrir;
    private float volGlobalAlAbrir;
    private float volMusicaAlAbrir;
    private float volEnemigosAlAbrir;
    private float volEfectosAlAbrir;

    void Start()
    {
        // 1. Cargar la configuración real del perfil nada más iniciar el juego
        CargarConfiguracionInicialDelPerfil();

        // 2. Escuchar los cambios que haga el usuario en la UI
        sliderGlobal.onValueChanged.AddListener(val => volGlobalTemporal = val);
        sliderMusica.onValueChanged.AddListener(val => volMusicaTemporal = val);
        sliderEnemigos.onValueChanged.AddListener(val => volEnemigosTemporal = val);
        sliderEfectos.onValueChanged.AddListener(val => volEfectosTemporal = val);

        if (togglePantallaCompleta != null)
        {
            togglePantallaCompleta.onValueChanged.AddListener(SeleccionarPantallaCompletaTemporal);
        }

        // 3. Configurar el click de la imagen de guardar automáticamente
        ConfigurarBotonGuardar();
    }

    private void CargarConfiguracionInicialDelPerfil()
    {
        volGlobalTemporal = PlayerPrefs.GetFloat("VolMaster", 0.75f);
        volMusicaTemporal = PlayerPrefs.GetFloat("VolMusica", 0.75f);
        volEnemigosTemporal = PlayerPrefs.GetFloat("VolEnemigos", 0.75f);
        volEfectosTemporal = PlayerPrefs.GetFloat("VolEfectos", 0.75f);
        estadoPantallaTemporal = (PlayerPrefs.GetInt("PantallaCompleta", 1) == 1);

        // Aplicar a los elementos visuales
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

        // Aplicar los volúmenes reales al Mixer al arrancar el juego
        SetMixerVolumeReal("VolMaster", volGlobalTemporal);
        SetMixerVolumeReal("VolMusica", volMusicaTemporal);
        SetMixerVolumeReal("VolEnemigos", volEnemigosTemporal);
        SetMixerVolumeReal("VolEfectos", volEfectosTemporal);
    }

    // Cada vez que se abre el panel, guardamos cómo estaba por si le da a Cancelar
    void OnEnable()
    {
        volGlobalAlAbrir = sliderGlobal.value;
        volMusicaAlAbrir = sliderMusica.value;
        volEnemigosAlAbrir = sliderEnemigos.value;
        volEfectosAlAbrir = sliderEfectos.value;
        if (togglePantallaCompleta != null) pantallaAlAbrir = togglePantallaCompleta.isOn;

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

    // --- BOTÓN GUARDAR: Guarda y sale al menú de pausa ---
    public void GuardarConfiguracion()
    {
        Debug.Log("¡Configuración guardada! Saliendo al menú de pausa...");

        // 1. Aplicamos los cambios al juego y los guardamos en el perfil
        Screen.fullScreen = estadoPantallaTemporal;
        PlayerPrefs.SetInt("PantallaCompleta", estadoPantallaTemporal ? 1 : 0);

        AplicarYGuardarVolumen("VolMaster", volGlobalTemporal);
        AplicarYGuardarVolumen("VolMusica", volMusicaTemporal);
        AplicarYGuardarVolumen("VolEnemigos", volEnemigosTemporal);
        AplicarYGuardarVolumen("VolEfectos", volEfectosTemporal);
        PlayerPrefs.Save(); // Forzar escritura en disco

        // 2. SALIR DEL MENÚ (Cierra ajustes y vuelve a la pausa)
        if (canvasAjustes != null) canvasAjustes.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(true);
    }

    // --- BOTÓN CANCELAR: Restaura los valores de antes de abrir y sale ---
    public void CancelarCambios()
    {
        Debug.Log("Restaurando valores anteriores y saliendo...");

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
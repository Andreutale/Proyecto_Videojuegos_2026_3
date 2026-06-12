using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    [Header("Botón de Guardar")]
    public Image btnGuardar; // Arrastra aquí tu 'btnGuardar'

    // Variables temporales para almacenar los cambios antes de guardar
    private bool estadoPantallaTemporal;
    private float volGlobalTemporal;
    private float volMusicaTemporal;
    private float volEnemigosTemporal;
    private float volEfectosTemporal;

    void Start()
    {
        // --- 1. CARGAR CONFIGURACIÓN ACTUAL AL ENTRAR ---
        volGlobalTemporal = PlayerPrefs.GetFloat("VolMaster", 0.75f);
        volMusicaTemporal = PlayerPrefs.GetFloat("VolMusica", 0.75f);
        volEnemigosTemporal = PlayerPrefs.GetFloat("VolEnemigos", 0.75f);
        volEfectosTemporal = PlayerPrefs.GetFloat("VolEfectos", 0.75f);

        int pantallaCompletaGuardada = PlayerPrefs.GetInt("PantallaCompleta", 1);
        estadoPantallaTemporal = (pantallaCompletaGuardada == 1);

        // --- 2. ASIGNAR VALORES VISUALES A LA UI ---
        sliderGlobal.value = volGlobalTemporal;
        sliderMusica.value = volMusicaTemporal;
        sliderEnemigos.value = volEnemigosTemporal;
        sliderEfectos.value = volEfectosTemporal;

        if (togglePantallaCompleta != null)
        {
            togglePantallaCompleta.onValueChanged.RemoveAllListeners();
            togglePantallaCompleta.isOn = estadoPantallaTemporal;
            ActualizarVisualCheckbox(estadoPantallaTemporal);

            // Cuando cambie el toggle, solo guardamos el valor temporalmente
            togglePantallaCompleta.onValueChanged.AddListener(SeleccionarPantallaCompletaTemporal);
        }

        // Cuando cambien los sliders, solo guardamos el valor temporalmente
        sliderGlobal.onValueChanged.AddListener(val => volGlobalTemporal = val);
        sliderMusica.onValueChanged.AddListener(val => volMusicaTemporal = val);
        sliderEnemigos.onValueChanged.AddListener(val => volEnemigosTemporal = val);
        sliderEfectos.onValueChanged.AddListener(val => volEfectosTemporal = val);

        // --- 3. ASIGNAR FUNCIÓN A LA IMAGEN DE GUARDAR ---
        if (btnGuardar != null)
        {
            // Nos aseguramos de que la imagen pueda recibir clics
            btnGuardar.raycastTarget = true;

            // Le añadimos el componente que detecta eventos si no lo tiene
            EventTrigger trigger = btnGuardar.gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = btnGuardar.gameObject.AddComponent<EventTrigger>();

            trigger.triggers.Clear();

            // Creamos el evento de "Hacer Clic" (PointerClick)
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { GuardarConfiguracion(); });

            trigger.triggers.Add(entry);
        }
    }

    // Cambia el aspecto visual de la checkbox inmediatamente para que el usuario vea que hizo clic
    public void SeleccionarPantallaCompletaTemporal(bool esCompleta)
    {
        estadoPantallaTemporal = esCompleta;
        ActualizarVisualCheckbox(esCompleta);
    }

    // ¡ESTA ES LA FUNCIÓN CLAVE! Se ejecuta solo al pulsar "btnGuardar"
    public void GuardarConfiguracion()
    {
        Debug.Log("Guardando configuración aplicada por el usuario...");

        // APLICAR Y GUARDAR PANTALLA COMPLETA
        Screen.fullScreen = estadoPantallaTemporal;
        PlayerPrefs.SetInt("PantallaCompleta", estadoPantallaTemporal ? 1 : 0);

        // APLICAR Y GUARDAR VOLÚMENES EN EL MIXER
        AplicarYGuardarVolumen("VolMaster", volGlobalTemporal);
        AplicarYGuardarVolumen("VolMusica", volMusicaTemporal);
        AplicarYGuardarVolumen("VolEnemigos", volEnemigosTemporal);
        AplicarYGuardarVolumen("VolEfectos", volEfectosTemporal);

        // Forzar el guardado físico en el disco duro/dispositivo
        PlayerPrefs.Save();
    }

    private void AplicarYGuardarVolumen(string nombreParametro, float valorSlider)
    {
        // Aplicamos al Mixer real
        if (valorSlider <= 0.001f)
        {
            audioMixer.SetFloat(nombreParametro, -80f);
        }
        else
        {
            audioMixer.SetFloat(nombreParametro, Mathf.Log10(valorSlider) * 20);
        }

        // Guardamos en los datos locales
        PlayerPrefs.SetFloat(nombreParametro, valorSlider);
    }

    private void ActualizarVisualCheckbox(bool esCompleta)
    {
        if (checkboxImagen != null && spriteActivado != null && spriteDesactivado != null)
        {
            checkboxImagen.sprite = esCompleta ? spriteActivado : spriteDesactivado;
        }
    }
}
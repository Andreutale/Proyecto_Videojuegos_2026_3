using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // Necesario para controlar el AudioMixer

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

    void Start()
    {
        // --- CARGAR VOLÚMENES ---
        CargarSlider(sliderGlobal, "VolMaster", 0.75f);
        CargarSlider(sliderMusica, "VolMusica", 0.75f);
        CargarSlider(sliderEnemigos, "VolEnemigos", 0.75f);
        CargarSlider(sliderEfectos, "VolEfectos", 0.75f);

        // --- CARGAR PANTALLA COMPLETA ---
        int pantallaCompletaGuardada = PlayerPrefs.GetInt("PantallaCompleta", 1);
        bool esPantallaCompleta = (pantallaCompletaGuardada == 1);
        togglePantallaCompleta.isOn = esPantallaCompleta;
        CambiarPantallaCompleta(esPantallaCompleta);

        // --- ASIGNAR ESCUCHADORES DE EVENTOS ---
        sliderGlobal.onValueChanged.AddListener(val => CambiarVolumenBase("VolMaster", val));
        sliderMusica.onValueChanged.AddListener(val => CambiarVolumenBase("VolMusica", val));
        sliderEnemigos.onValueChanged.AddListener(val => CambiarVolumenBase("VolEnemigos", val));
        sliderEfectos.onValueChanged.AddListener(val => CambiarVolumenBase("VolEfectos", val));
        togglePantallaCompleta.onValueChanged.AddListener(CambiarPantallaCompleta);
    }

    // Función auxiliar para inicializar sliders sin repetir código
    private void CargarSlider(Slider slider, string nombrePref, float valorPorDefecto)
    {
        float volGuardado = PlayerPrefs.GetFloat(nombrePref, valorPorDefecto);
        slider.value = volGuardado;
        SetMixerVolume(nombrePref, volGuardado);
    }

    // Modifica el volumen en el Mixer y guarda la preferencia
    private void CambiarVolumenBase(string nombreParametro, float valorSlider)
    {
        SetMixerVolume(nombreParametro, valorSlider);
        PlayerPrefs.SetFloat(nombreParametro, valorSlider);
    }

    // Convierte el valor del slider (0 a 1) a Decibelios (-80 a 0) de forma logarítmica
    private void SetMixerVolume(string nombreParametro, float valorSlider)
    {
        if (valorSlider <= 0.001f)
        {
            audioMixer.SetFloat(nombreParametro, -80f); // Silencio total si el slider está al mínimo
        }
        else
        {
            // Fórmula matemática para que la percepción del oído al mover el slider sea natural
            audioMixer.SetFloat(nombreParametro, Mathf.Log10(valorSlider) * 20);
        }
    }

    public void CambiarPantallaCompleta(bool esCompleta)
    {
        Screen.fullScreen = esCompleta;
        PlayerPrefs.SetInt("PantallaCompleta", esCompleta ? 1 : 0);

        if (checkboxImagen != null && spriteActivado != null && spriteDesactivado != null)
        {
            checkboxImagen.sprite = esCompleta ? spriteActivado : spriteDesactivado;
        }
    }
}
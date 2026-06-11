using UnityEngine;
using UnityEngine.UI;

public class ConfiguracionMenu : MonoBehaviour
{
    [Header("Componentes de UI")]
    public Slider sliderVolumen;
    public Toggle togglePantallaCompleta;

    [Header("Imágenes Personalizadas")]
    public Image checkboxImagen;
    public Sprite spriteActivado;
    public Sprite spriteDesactivado;

    void Start()
    {
        // 1. Cargar la configuración guardada del volumen (por defecto 0.5f si no existe)
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenAudio", 0.5f);
        sliderVolumen.value = volumenGuardado;
        CambiarVolumen(volumenGuardado);

        // 2. Cargar la configuración de pantalla completa (1 = Sí, 0 = No. Por defecto Sí)
        int pantallaCompletaGuardada = PlayerPrefs.GetInt("PantallaCompleta", 1);
        bool esPantallaCompleta = (pantallaCompletaGuardada == 1);
        togglePantallaCompleta.isOn = esPantallaCompleta;
        CambiarPantallaCompleta(esPantallaCompleta);

        // Añadir los escuchadores por código para evitar errores de asignación manual
        sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
        togglePantallaCompleta.onValueChanged.AddListener(CambiarPantallaCompleta);
    }

    public void CambiarVolumen(float valor)
    {
        // Modifica el volumen general del juego
        AudioListener.volume = valor;

        // Guarda el valor para la próxima vez que se abra el juego
        PlayerPrefs.SetFloat("VolumenAudio", valor);
    }

    public void CambiarPantallaCompleta(bool esCompleta)
    {
        // Cambia el modo de pantalla en el juego compilado
        Screen.fullScreen = esCompleta;

        // Guarda la preferencia (1 para verdadero, 0 para falso)
        PlayerPrefs.SetInt("PantallaCompleta", esCompleta ? 1 : 0);

        // Cambia visualmente tu imagen personalizada de la checkbox
        if (checkboxImagen != null && spriteActivado != null && spriteDesactivado != null)
        {
            checkboxImagen.sprite = esCompleta ? spriteActivado : spriteDesactivado;
        }
    }
}
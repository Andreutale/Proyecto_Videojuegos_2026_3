using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EfectoBrilloPuerta : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Referencias")]
    public Image imagenFondo;

    [Header("Configuración de Brillo")]
    public Color colorNormal = Color.white;
    public Color colorBrillo = new Color(1f, 1f, 0.6f, 1f);
    public float velocidadTransicion = 5f;

    private Color colorObjetivo;
    private bool esInteractuable = true;

    void Start()
    {
        colorObjetivo = colorNormal;
        Button btn = GetComponent<Button>();
        if (btn != null && !btn.interactable)
            esInteractuable = false;
    }

    void Update()
    {
        if (imagenFondo == null || !esInteractuable) return;
        imagenFondo.color = Color.Lerp(imagenFondo.color, colorObjetivo, Time.deltaTime * velocidadTransicion);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!esInteractuable) return;
        colorObjetivo = colorBrillo;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        colorObjetivo = colorNormal;
    }
}
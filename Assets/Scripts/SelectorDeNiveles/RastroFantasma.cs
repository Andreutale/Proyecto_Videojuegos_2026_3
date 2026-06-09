using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RastroFantasma : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform contenedorRastro;

    [Header("Configuración de Rastro")]
    public float intervaloCreacion = 0.05f;
    public float duracionRastro = 0.4f;
    public Color colorRastro = new Color(0.4f, 0.8f, 1f, 0.6f);

    private Image imagenFantasma;
    private bool estaMoviendo = false;
    private Coroutine corrutinaRastro;

    void Awake()
    {
        imagenFantasma = GetComponent<Image>();
    }

    public void IniciarRastro()
    {
        estaMoviendo = true;
        corrutinaRastro = StartCoroutine(GenerarRastro());
    }

    public void DetenerRastro()
    {
        estaMoviendo = false;
        if (corrutinaRastro != null)
            StopCoroutine(corrutinaRastro);
    }

    private IEnumerator GenerarRastro()
    {
        while (estaMoviendo)
        {
            CrearFragmentoRastro();
            yield return new WaitForSeconds(intervaloCreacion);
        }
    }

    private void CrearFragmentoRastro()
    {
        if (contenedorRastro == null || imagenFantasma == null) return;

        GameObject fragmento = new GameObject("Rastro");
        fragmento.transform.SetParent(contenedorRastro, false);

        Image imgRastro = fragmento.AddComponent<Image>();
        imgRastro.sprite = imagenFantasma.sprite;
        imgRastro.color = colorRastro;

        RectTransform rt = fragmento.GetComponent<RectTransform>();
        RectTransform rtFantasma = GetComponent<RectTransform>();
        rt.sizeDelta = rtFantasma.sizeDelta;
        rt.anchoredPosition = rtFantasma.anchoredPosition;

        StartCoroutine(SolventarRastro(imgRastro, fragmento));
    }

    private IEnumerator SolventarRastro(Image img, GameObject obj)
    {
        float t = 0f;
        Color colorInicial = img.color;

        while (t < duracionRastro)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(colorInicial.a, 0f, t / duracionRastro);
            img.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, alpha);
            yield return null;
        }

        Destroy(obj);
    }
}
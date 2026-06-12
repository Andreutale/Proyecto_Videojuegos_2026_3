using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimacionDesbloqueo : MonoBehaviour
{
    [Header("Referencias - Botón")]
    public RectTransform botonPiso;
    public Image fondoOscuro;
    public Image imagenCandado;

    [Header("Referencias - Partículas")]
    public RectTransform contenedorParticulas;
    public Sprite spriteParticula;

    [Header("Configuración - Zoom")]
    public float duracionZoomEntrada = 0.5f;
    public float escalaCentro = 2.5f;
    public float duracionZoomSalida = 0.4f;

    [Header("Configuración - Sacudida")]
    public float duracionSacudida = 0.6f;
    public float intensidadSacudida = 12f;
    public float frecuenciaSacudida = 25f;

    [Header("Configuración - Partículas")]
    public int cantidadParticulas = 20;
    public float velocidadParticula = 300f;
    public float duracionParticula = 0.7f;
    public Color colorParticula1 = new Color(1f, 0.85f, 0.2f, 1f);
    public Color colorParticula2 = new Color(1f, 0.4f, 0.1f, 1f);

    [Header("Audio")]
    [SerializeField] private AudioClip sonidoDesbloqueo;

    private Vector2 posicionOriginal;
    private Vector3 escalaOriginal;
    private bool yaReproducido = false;

    void Start()
    {
        if (botonPiso == null) return;

        posicionOriginal = botonPiso.anchoredPosition;
        escalaOriginal = botonPiso.localScale;

        BotonPiso boton = GetComponent<BotonPiso>();
        if (boton == null) return;

        bool estaDesbloqueado = !boton.estaBloqueado;
        int animacionVista = PlayerPrefs.GetInt(boton.nombreNivel + "_AnimVista", 0);

        if (estaDesbloqueado && animacionVista == 0 && !yaReproducido)
        {
            if (imagenCandado != null) imagenCandado.gameObject.SetActive(true);
            if (fondoOscuro != null) fondoOscuro.gameObject.SetActive(true);
            PlayerPrefs.SetInt(boton.nombreNivel + "_AnimVista", 1);
            PlayerPrefs.Save();
            StartCoroutine(SecuenciaDesbloqueo());
        }
        else if (estaDesbloqueado)
        {
            if (imagenCandado != null) imagenCandado.gameObject.SetActive(false);
            if (fondoOscuro != null) fondoOscuro.gameObject.SetActive(false);
        }
    }
    private IEnumerator SecuenciaDesbloqueo()
    {
        yaReproducido = true;

        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(MoverAlCentro());
        yield return StartCoroutine(Sacudir());
        yield return StartCoroutine(RomperCandado());
        yield return StartCoroutine(VolverAPosicion());
    }

    private IEnumerator MoverAlCentro()
    {
        float t = 0f;
        Vector2 posInicial = botonPiso.anchoredPosition;
        Vector3 escalaInicial = botonPiso.localScale;
        Vector3 escalaDestino = escalaOriginal * escalaCentro;

        botonPiso.SetAsLastSibling();

        while (t < duracionZoomEntrada)
        {
            t += Time.deltaTime;
            float progreso = Mathf.SmoothStep(0f, 1f, t / duracionZoomEntrada);
            botonPiso.anchoredPosition = Vector2.Lerp(posInicial, Vector2.zero, progreso);
            botonPiso.localScale = Vector3.Lerp(escalaInicial, escalaDestino, progreso);
            yield return null;
        }

        botonPiso.anchoredPosition = Vector2.zero;
        botonPiso.localScale = escalaDestino;
    }

    private IEnumerator Sacudir()
    {

        if (sonidoDesbloqueo != null)
            SFXManager.Instance.PlaySFX(sonidoDesbloqueo, transform, 1f);

        float t = 0f;

        while (t < duracionSacudida)
        {
            t += Time.deltaTime;
            float progreso = t / duracionSacudida;
            float amplitud = intensidadSacudida * (1f - progreso);
            float offsetX = Mathf.Sin(t * frecuenciaSacudida) * amplitud;
            float offsetY = Mathf.Cos(t * frecuenciaSacudida * 0.7f) * amplitud * 0.5f;
            botonPiso.anchoredPosition = new Vector2(offsetX, offsetY);
            yield return null;
        }

        botonPiso.anchoredPosition = Vector2.zero;
    }

    private IEnumerator RomperCandado()
    {
        ExplotarParticulas();

        if (imagenCandado != null)
        {
            float t = 0f;
            float duracion = 0.4f;
            Vector3 escalaInicialCandado = imagenCandado.rectTransform.localScale;
            Vector3 escalaFinalCandado = escalaInicialCandado * 2f;

            while (t < duracion)
            {
                t += Time.deltaTime;
                float progreso = t / duracion;
                imagenCandado.rectTransform.localScale = Vector3.Lerp(escalaInicialCandado, escalaFinalCandado, progreso);
                imagenCandado.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, progreso));
                yield return null;
            }

            imagenCandado.gameObject.SetActive(false);
        }

        if (fondoOscuro != null)
        {
            float t = 0f;
            float duracion = 0.4f;
            Color colorInicial = fondoOscuro.color;

            while (t < duracion)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(colorInicial.a, 0f, t / duracion);
                fondoOscuro.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, alpha);
                yield return null;
            }

            fondoOscuro.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.3f);
    }

    private void ExplotarParticulas()
    {
        if (contenedorParticulas == null) return;

        for (int i = 0; i < cantidadParticulas; i++)
        {
            GameObject p = new GameObject("Particula");
            p.transform.SetParent(contenedorParticulas, false);

            Image img = p.AddComponent<Image>();
            if (spriteParticula != null)
                img.sprite = spriteParticula;

            img.color = Random.value > 0.5f ? colorParticula1 : colorParticula2;

            RectTransform rt = p.GetComponent<RectTransform>();
            float tamanio = Random.Range(6f, 14f);
            rt.sizeDelta = new Vector2(tamanio, tamanio);
            rt.anchoredPosition = Vector2.zero;

            Vector2 direccion = Random.insideUnitCircle.normalized;
            StartCoroutine(AnimarParticula(rt, img, direccion));
        }
    }

    private IEnumerator AnimarParticula(RectTransform rt, Image img, Vector2 direccion)
    {
        float t = 0f;
        Color colorInicial = img.color;

        while (t < duracionParticula)
        {
            t += Time.deltaTime;
            float progreso = t / duracionParticula;
            rt.anchoredPosition += direccion * velocidadParticula * Time.deltaTime * (1f - progreso);
            img.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b,
                                  Mathf.Lerp(1f, 0f, progreso));
            yield return null;
        }

        Destroy(rt.gameObject);
    }

    private IEnumerator VolverAPosicion()
    {
        float t = 0f;
        Vector2 posActual = botonPiso.anchoredPosition;
        Vector3 escalaActual = botonPiso.localScale;

        while (t < duracionZoomSalida)
        {
            t += Time.deltaTime;
            float progreso = Mathf.SmoothStep(0f, 1f, t / duracionZoomSalida);
            botonPiso.anchoredPosition = Vector2.Lerp(posActual, posicionOriginal, progreso);
            botonPiso.localScale = Vector3.Lerp(escalaActual, escalaOriginal, progreso);
            yield return null;
        }

        botonPiso.anchoredPosition = posicionOriginal;
        botonPiso.localScale = escalaOriginal;
    }
}
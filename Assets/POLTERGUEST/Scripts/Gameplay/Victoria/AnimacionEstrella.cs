using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimacionEstrella : MonoBehaviour
{
    [Header("Animación Pop")]
    public float duracion = 0.5f;
    public float escalaMaxima = 1.6f;
    public float retraso = 0f;

    [Header("Brillo")]
    public Image imagenEstrella;
    public Color colorNormal = new Color(1f, 0.92f, 0.2f, 1f);
    public Color colorBrillo = new Color(1f, 1f, 1f, 1f);
    public float duracionBrillo = 0.5f;

    [Header("Partículas UI")]
    public EfectoParticulasUI efectoParticulas;

    private Vector3 escalaFinal = Vector3.one;

    private void Awake()
    {
        if (imagenEstrella == null)
            imagenEstrella = GetComponent<Image>();
    }

    public void Activar()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        transform.localScale = Vector3.zero;

        if (imagenEstrella != null)
            imagenEstrella.color = new Color(
                colorNormal.r, colorNormal.g, colorNormal.b, 0f);

        StartCoroutine(Animar());
    }

    private IEnumerator Animar()
    {
        yield return new WaitForSecondsRealtime(retraso);

        StartCoroutine(AnimacionPop());
        StartCoroutine(Brillar());

        if (efectoParticulas != null)
            efectoParticulas.Reproducir();
    }

    private IEnumerator AnimacionPop()
    {
        float fase1 = duracion * 0.55f;
        float tiempo = 0f;
        while (tiempo < fase1)
        {
            tiempo += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(tiempo / fase1);
            float escala = Mathf.Lerp(0f, escalaMaxima, EaseOutExpo(t));
            transform.localScale = Vector3.one * escala;
            yield return null;
        }

        float fase2 = duracion * 0.25f;
        tiempo = 0f;
        while (tiempo < fase2)
        {
            tiempo += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(tiempo / fase2);
            transform.localScale = Vector3.Lerp(
                Vector3.one * escalaMaxima, Vector3.one * 1.05f, t);
            yield return null;
        }

        float fase3 = duracion * 0.2f;
        tiempo = 0f;
        while (tiempo < fase3)
        {
            tiempo += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(tiempo / fase3);
            transform.localScale = Vector3.Lerp(
                Vector3.one * 1.05f, escalaFinal, t);
            yield return null;
        }

        transform.localScale = escalaFinal;
    }

    private IEnumerator Brillar()
    {
        if (imagenEstrella == null) yield break;

        Color transparent = new Color(colorNormal.r, colorNormal.g, colorNormal.b, 0f);
        float tercio = duracionBrillo / 3f;
        float tiempo = 0f;

        while (tiempo < tercio)
        {
            tiempo += Time.unscaledDeltaTime;
            imagenEstrella.color = Color.Lerp(
                transparent, colorBrillo, tiempo / tercio);
            yield return null;
        }

        tiempo = 0f;
        while (tiempo < tercio * 2f)
        {
            tiempo += Time.unscaledDeltaTime;
            imagenEstrella.color = Color.Lerp(
                colorBrillo, colorNormal, tiempo / (tercio * 2f));
            yield return null;
        }

        imagenEstrella.color = colorNormal;
    }

    private float EaseOutExpo(float t)
    {
        return t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
    }
}
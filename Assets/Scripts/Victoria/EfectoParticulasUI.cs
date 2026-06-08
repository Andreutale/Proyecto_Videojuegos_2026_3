using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EfectoParticulasUI : MonoBehaviour
{
    [Header("Configuración")]
    public int cantidadParticulas = 8;
    public float radio = 60f;
    public float duracion = 0.5f;
    public Color colorParticula = new Color(1f, 0.95f, 0.3f, 1f);
    public float tamanoParticula = 12f;

    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void Reproducir()
    {
        StartCoroutine(SpawnParticulas());
    }

    private IEnumerator SpawnParticulas()
    {
        for (int i = 0; i < cantidadParticulas; i++)
        {
            float angulo = (360f / cantidadParticulas) * i;
            Vector2 dir = new Vector2(
                Mathf.Cos(angulo * Mathf.Deg2Rad),
                Mathf.Sin(angulo * Mathf.Deg2Rad));

            StartCoroutine(AnimarParticula(dir));
        }
        yield return null;
    }

    private IEnumerator AnimarParticula(Vector2 direccion)
    {
        GameObject obj = new GameObject("Particula");
        obj.transform.SetParent(transform, false);

        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta = Vector2.one * tamanoParticula;
        rt.anchoredPosition = Vector2.zero;

        Image img = obj.AddComponent<Image>();
        img.color = colorParticula;
        img.raycastTarget = false;

        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(tiempo / duracion);

            rt.anchoredPosition = direccion * radio * EaseOut(t);

            float alpha = 1f - t;
            img.color = new Color(
                colorParticula.r,
                colorParticula.g,
                colorParticula.b,
                alpha);

            float escala = Mathf.Lerp(1f, 0.2f, t);
            rt.localScale = Vector3.one * escala;

            yield return null;
        }

        Destroy(obj);
    }

    private float EaseOut(float t)
    {
        return 1f - Mathf.Pow(1f - t, 2f);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AnimacionPuerta : MonoBehaviour
{
    public string animAbrirPuerta = "AbrirPuerta";
    public int indiceSala;
    public RectTransform fantasmaUI;
    public RectTransform destinoUI;

    private bool reproduciendo = false;

    public void EntrarPuerta()
    {
        if (!reproduciendo)
            StartCoroutine(AnimacionYCarga());
    }

    private IEnumerator AnimacionYCarga()
    {
        reproduciendo = true;

        Animator anim = GetComponent<Animator>();
        if (anim == null)
            anim = GetComponentInChildren<Animator>(true);
        if (anim != null)
            anim.SetTrigger("Abrir");

        yield return new WaitForSeconds(1f);

        if (fantasmaUI != null && destinoUI != null)
        {
            float duracion = 2f;
            float tiempoTranscurrido = 0f;
            Vector2 posicionInicial = fantasmaUI.anchoredPosition;
            Vector2 posicionDestino = destinoUI.anchoredPosition;


            while (tiempoTranscurrido < duracion)
            {
                tiempoTranscurrido += Time.deltaTime;
                fantasmaUI.anchoredPosition = Vector2.Lerp(posicionInicial, posicionDestino, tiempoTranscurrido / duracion);
                yield return null;
            }

            fantasmaUI.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(indiceSala);
    }
}
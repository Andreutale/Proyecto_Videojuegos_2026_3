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

        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
            anim.SetTrigger("Abrir");

        yield return new WaitForSeconds(1f);

        if (fantasmaUI != null && destinoUI != null)
        {
            float sure = 2f;
            float gecenSure = 0f;
            Vector2 baslangic = fantasmaUI.anchoredPosition;
            Vector2 destino = destinoUI.anchoredPosition;

            while (gecenSure < sure)
            {
                gecenSure += Time.deltaTime;
                fantasmaUI.anchoredPosition = Vector2.Lerp(baslangic, destino, gecenSure / sure);
                yield return null;
            }

            fantasmaUI.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(indiceSala);
    }
}
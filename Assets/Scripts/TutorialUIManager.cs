using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Tutorial
{
    /// <summary>
    /// Gestiona el panel de UI que muestra el texto de los carteles de tutorial.
    ///
    /// SETUP EN UNITY:
    ///   1. En tu Canvas principal, crea un Panel (clic derecho en Canvas → UI → Panel)
    ///      Llámalo "TutorialPanel"
    ///   2. Dentro del Panel añade:
    ///        - Image "Fondo" (un cuadro semi-transparente)
    ///        - TextMeshPro "Titulo" (texto grande arriba)
    ///        - TextMeshPro "Texto" (texto explicativo debajo)
    ///        - Image "Icono" (opcional, para el icono de la habilidad)
    ///   3. Añade este script al GameObject "TutorialPanel"
    ///   4. Arrastra cada elemento a su campo correspondiente en el Inspector
    ///   5. Desactiva el Panel por defecto (el script lo activa solo)
    ///   6. En cada TutorialSign de la escena, arrastra este GameObject
    ///      al campo "Tutorial UI"
    /// </summary>
    public class TutorialUIManager : MonoBehaviour
    {
        [Header("Referencias UI")]
        [SerializeField] private GameObject panelRaiz;       // El panel completo (con CanvasGroup)
        [SerializeField] private TextMeshProUGUI textoTitulo;
        [SerializeField] private TextMeshProUGUI textoCuerpo;
        [SerializeField] private Image imagenIcono;

        [Header("Animación")]
        [SerializeField] private float fadeDuration = 0.25f;

        private CanvasGroup canvasGroup;
        private Coroutine fadeCoroutine;

        private void Awake()
        {
            if (panelRaiz == null) panelRaiz = gameObject;

            canvasGroup = panelRaiz.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = panelRaiz.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0f;
            panelRaiz.SetActive(false);
        }

        public void MostrarTutorial(string titulo, string texto, Sprite icono = null)
        {
            if (textoTitulo != null) textoTitulo.text = titulo;
            if (textoCuerpo != null) textoCuerpo.text = texto;

            if (imagenIcono != null)
            {
                if (icono != null)
                {
                    imagenIcono.sprite = icono;
                    imagenIcono.gameObject.SetActive(true);
                }
                else
                {
                    imagenIcono.gameObject.SetActive(false);
                }
            }

            panelRaiz.SetActive(true);

            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(Fade(1f));
        }

        public void OcultarTutorial()
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(Fade(0f));
        }

        private IEnumerator Fade(float target)
        {
            float start = canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, target, elapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = target;

            if (target <= 0f)
                panelRaiz.SetActive(false);
        }
    }
}
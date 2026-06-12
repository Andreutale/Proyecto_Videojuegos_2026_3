

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tutorial
{
    /// <summary>
    /// Cartel de tutorial. Colócalo en la escena con un Collider (Is Trigger = true)
    /// del tamaño del área donde quieres que aparezca el texto.
    ///
    /// SETUP:
    ///   1. Crea un GameObject vacío, ponle un BoxCollider con "Is Trigger" activado.
    ///   2. Añade este script.
    ///   3. Escribe el título y el texto del tutorial en el Inspector.
    ///   4. Asigna el "Tutorial Canvas" - ver TutorialUIManager.cs para crear el Canvas.
    ///   5. Asegúrate de que el jugador tiene Tag "Player".
    /// </summary>
    public class TutorialSign : MonoBehaviour
    {
        [Header("Contenido del Tutorial")]
        [Tooltip("Título corto, ej: 'Posesión'")]
        [SerializeField] private string titulo = "Posesión";

        [TextArea(3, 6)]
        [Tooltip("Texto explicativo de la mecánica")]
        [SerializeField] private string texto = "Pulsa Z para escanear objetos cercanos.\nVuelve a pulsar Z para poseerlos.";

        [Header("Icono (opcional)")]
        [Tooltip("Icono de la habilidad a mostrar junto al texto")]
        [SerializeField] private Sprite icono;

        [Header("Referencias")]
        [Tooltip("Arrastra aquí el TutorialUIManager de la escena")]
        [SerializeField] private TutorialUIManager tutorialUI;

        [Header("Opciones")]
        [Tooltip("Si está activo, el cartel solo se muestra una vez por partida")]
        [SerializeField] private bool soloUnaVez = false;

        private bool yaMostrado = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (soloUnaVez && yaMostrado) return;

            if (tutorialUI != null)
            {
                tutorialUI.MostrarTutorial(titulo, texto, icono);
                yaMostrado = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (soloUnaVez) return; // si es de una sola vez, no lo ocultamos al salir

            if (tutorialUI != null)
                tutorialUI.OcultarTutorial();
        }

        // Para ver el área del cartel en la Scene view
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.3f, 0.8f, 1f, 0.3f);
            Collider col = GetComponent<Collider>();
            if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }
        }
    }
}
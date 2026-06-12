using UnityEngine;

public class TutorialSimple : MonoBehaviour
{
    [Header("Configuración de Paneles")]
    // Arrastra aquí tus paneles en orden desde el inspector de Unity
    public GameObject[] panelesTutorial;

    private int indiceActual = 0;

    void Start()
    {
        // Al iniciar el juego, nos aseguramos de que el tutorial esté apagado
        // para que no tape vuestra cuenta atrás de 3, 2, 1...
        if (panelesTutorial != null && panelesTutorial.Length > 0)
        {
            panelesTutorial[0].SetActive(false);
        }
    }

    // Esta es la función que activa la cuenta atrás en cuanto dice "¡EMPIEZA!"
    public void ActivarPrimerPanel()
    {
        Debug.Log("TutorialSimple: Recibida la orden de activar el primer panel.");

        if (panelesTutorial != null && panelesTutorial.Length > 0)
        {
            if (panelesTutorial[0] != null)
            {
                panelesTutorial[0].SetActive(true);
                Debug.Log("TutorialSimple: ¡Primer panel activado con éxito!");
            }
            else
            {
                Debug.LogError("TutorialSimple: El Element 0 de la lista está vacío en el Inspector.");
            }
        }
        else
        {
            Debug.LogError("TutorialSimple: La lista 'panelesTutorial' está vacía. Añade el tamaño y tus paneles.");
        }
    }

    // Esta es la función que tenéis asignada en los botones "Siguiente"
    public void AvanzarTutorial()
    {
        if (indiceActual < panelesTutorial.Length && panelesTutorial[indiceActual] != null)
        {
            // Desactiva el panel actual
            panelesTutorial[indiceActual].SetActive(false);
        }

        // Pasa al siguiente índice
        indiceActual++;

        // Si quedan más paneles en la lista, muestra el que toca
        if (indiceActual < panelesTutorial.Length)
        {
            if (panelesTutorial[indiceActual] != null)
            {
                panelesTutorial[indiceActual].SetActive(true);
            }
        }
        else
        {
            // Si ya no quedan más paneles (era el último botón), el tutorial se cierra por completo
            Debug.Log("¡Tutorial Terminado!");
        }
    }
}
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string mensaje;
    public float duracion = 5f;

    private bool activado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activado) return;

        if (other.CompareTag("Player"))
        {
            activado = true;

            FindObjectOfType<TutorialManager>()
                .MostrarMensaje(mensaje, duracion);
        }
    }
}

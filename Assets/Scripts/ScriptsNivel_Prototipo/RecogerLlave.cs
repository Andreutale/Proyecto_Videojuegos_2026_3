using UnityEngine;

public class RecogerLlave : MonoBehaviour
{
    private bool recogida = false;

    [Header("Audio")]
    public AudioClip sonidoLlave;
    [Range(0f, 1f)] public float volumen = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (recogida) return;

        if (other.CompareTag("Player"))
        {
            recogida = true;

            if (KeyCounterUI.Instance != null)
                KeyCounterUI.Instance.AddKey();

            if (GameManager.Instance != null)
                GameManager.Instance.RecogerLlave();

            if (sonidoLlave != null && Camera.main != null)
                AudioSource.PlayClipAtPoint(sonidoLlave, transform.position, 1f);

            Destroy(gameObject);
        }
    }
}
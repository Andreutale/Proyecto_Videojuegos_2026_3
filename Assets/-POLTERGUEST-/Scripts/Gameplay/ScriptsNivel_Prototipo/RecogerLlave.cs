using UnityEngine;

public class RecogerLlave : MonoBehaviour
{
    private bool recogida = false;

    [Header("Audio")]
    [SerializeField] private AudioClip sonidoLlave;

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

            if (sonidoLlave != null)
                SFXManager.Instance.PlaySFX(sonidoLlave, other.transform, 1f);

            Destroy(gameObject);
        }
    }
}
using UnityEngine;

public class GhostTelekinesisAnimation : MonoBehaviour
{
    [SerializeField] private Transform ghostModel;

    [Header("Gesto telequinesis")]
    [SerializeField] private float duration = 0.35f;
    [SerializeField] private float scalePulse = 0.08f;
    [SerializeField] private float tiltForward = 12f;

    private Vector3 originalScale;
    private Quaternion originalRotation;
    private bool isAnimating;
    private float timer;

    private void Start()
    {
        if (ghostModel == null)
            ghostModel = transform;

        originalScale = ghostModel.localScale;
        originalRotation = ghostModel.localRotation;
    }

    private void Update()
    {
        if (!isAnimating) return;

        timer += Time.deltaTime;
        float t = timer / duration;
        float curve = Mathf.Sin(t * Mathf.PI);

        ghostModel.localRotation = originalRotation * Quaternion.Euler(
            -tiltForward * curve,
            0f,
            0f
        );

        ghostModel.localScale = originalScale * (1f + scalePulse * curve);

        if (timer >= duration)
        {
            isAnimating = false;
            ghostModel.localRotation = originalRotation;
            ghostModel.localScale = originalScale;
        }
    }

    public void Play()
    {
        Debug.Log("Animación llamada en: " + gameObject.name + " | Modelo: " + ghostModel.name);
        timer = 0f;
        isAnimating = true;
    }
}
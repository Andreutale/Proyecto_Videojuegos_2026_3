using UnityEngine;

public class GhostEffects : MonoBehaviour
{
    [Header("Referencias")]
    public ParticleSystem particulasIdle;
    public TrailRenderer rastroMovimiento;

    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        float velocidad = animator != null ? animator.GetFloat("Speed") : 0f;
        bool estaMoviendo = velocidad > 0.25f;

        if (!particulasIdle.isPlaying)
            particulasIdle.Play();

        rastroMovimiento.emitting = estaMoviendo;
    }
}
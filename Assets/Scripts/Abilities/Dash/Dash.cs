using UnityEngine;
using System.Collections;

public class Dash : MonoBehaviour
{
    [Header("Referencias")]
    public Transform playerCam;
    private Rigidbody rb;
    private CharacterController controller;

    [Header("Ajustes de Dash")]
    public float dashForce = 25f;
    public float dashDuration = 0.25f;

    [Header("Efecto Trail Dash")]
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private float trailTimeNormal = 0.25f;
    [SerializeField] private float trailTimeDash = 0.10f;
    [SerializeField] private float trailWidthNormal = 0.15f;
    [SerializeField] private float trailWidthDash = 0.35f;

    [Header("Partículas Dash")]
    [SerializeField] private ParticleSystem psIdle;

    [SerializeField] private Animator animator;

    private Color psColorNormal;
    private float psSpeedNormal;
    private float psSizeNormal;
    private float psRateNormal;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
        }

        if (psIdle != null)
        {
            var main = psIdle.main;
            psColorNormal = main.startColor.color;
            psSpeedNormal = main.startSpeed.constant;
            psSizeNormal = main.startSize.constant;

            var emission = psIdle.emission;
            psRateNormal = emission.rateOverTime.constant;
        }
    }

    public void ExecuteDash()
    {
        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        Debug.Log("DASH ACTIVADO");

        ActivarTrailDash();

        if (controller != null) controller.enabled = false;
        yield return null;

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.linearVelocity = Vector3.zero;

        Vector3 direction = GetDirection();
        rb.AddForce(direction * dashForce, ForceMode.Impulse);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;

        yield return new WaitForFixedUpdate();

        if (controller != null) controller.enabled = true;

        DesactivarTrailDash();
    }

    private void ActivarTrailDash()
    {
        if (animator != null)
        {
            animator.SetTrigger("Dash");
        }

        if (trail != null)
        {
            trail.Clear();
            trail.emitting = true;

            trail.startColor = Color.yellow;
            trail.endColor = new Color(1f, 1f, 0f, 0f);

            trail.time = trailTimeDash;
            trail.startWidth = trailWidthDash;
            trail.endWidth = 0f;
        }

        if (psIdle != null)
        {
            var main = psIdle.main;
            main.startColor = new ParticleSystem.MinMaxGradient(Color.yellow);
            main.startSpeed = 2.5f;
            main.startSize = 0.08f;

            var emission = psIdle.emission;
            emission.rateOverTime = 45f;

            psIdle.Clear();
            psIdle.Play();
        }
        else
        {
            Debug.LogWarning("PS_Idle no está asignado en Dash");
        }

        Debug.Log("Trail y partículas dash activados");
    }

    private void DesactivarTrailDash()
    {
        if (trail != null)
        {
            trail.startColor = Color.cyan;
            trail.endColor = new Color(0f, 1f, 1f, 0f);

            trail.time = trailTimeNormal;
            trail.startWidth = trailWidthNormal;
            trail.endWidth = 0f;
        }

        if (psIdle != null)
        {
            var main = psIdle.main;
            main.startColor = new ParticleSystem.MinMaxGradient(psColorNormal);
            main.startSpeed = psSpeedNormal;
            main.startSize = psSizeNormal;

            var emission = psIdle.emission;
            emission.rateOverTime = psRateNormal;

            psIdle.Clear();
            psIdle.Play();
        }
    }

    private Vector3 GetDirection()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = playerCam.forward;
        Vector3 right = playerCam.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        if (h == 0 && v == 0) return forward;

        return (forward * v + right * h).normalized;
    }
}
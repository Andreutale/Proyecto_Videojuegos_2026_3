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

    [Header("Trail Dash")]
    [SerializeField] private GameObject trailDashObject;

    [Header("Partículas Dash")]
    [SerializeField] private ParticleSystem psIdle;

    [Header("Animación Dash")]
    [SerializeField] private Animator animator;

    [Header("Luz Dash")]
    [SerializeField] private Light luzDash;

    private Color colorLuzNormal;
    private float intensidadNormal;
    private float rangeNormal;

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

        if (trailDashObject != null)
        {
            trailDashObject.SetActive(false);
        }

        if (luzDash != null)
        {
            colorLuzNormal = luzDash.color;
            intensidadNormal = luzDash.intensity;
            rangeNormal = luzDash.range;
        }
    }

    public void ExecuteDash()
    {
        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        ActivarTrailDash();

        if (controller != null)
            controller.enabled = false;

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

        if (controller != null)
            controller.enabled = true;

        DesactivarTrailDash();
    }

    private void ActivarTrailDash()
    {
        if (animator != null)
        {
            animator.SetTrigger("Dash");
        }

        if (trailDashObject != null)
        {
            trailDashObject.SetActive(true);
        }

        if (psIdle != null)
        {
            var main = psIdle.main;

            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(1f, 0.85f, 0f, 1f)
            );

            main.startSpeed = 2.5f;
            main.startSize = 0.08f;

            var emission = psIdle.emission;
            emission.rateOverTime = 45f;

            psIdle.Clear();
            psIdle.Play();
        }
        if (luzDash != null)
        {
            luzDash.color = new Color(1f, 0.95f, 0.15f, 1f);

            luzDash.intensity = 5f;

            luzDash.range = 2.5f;
        }
    }

    private void DesactivarTrailDash()
    {
        if (trailDashObject != null)
        {
            trailDashObject.SetActive(false);
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
        if (luzDash != null)
        {
            luzDash.color = colorLuzNormal;

            luzDash.intensity = intensidadNormal;

            luzDash.range = rangeNormal;
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

        if (h == 0 && v == 0)
            return forward;

        return (forward * v + right * h).normalized;
    }
}
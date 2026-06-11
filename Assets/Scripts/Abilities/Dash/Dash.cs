using UnityEngine;
using System.Collections;

public class Dash : MonoBehaviour
{
    [Header("Referencias")]
    public Transform playerCam;
    private CharacterController controller;

    [Header("Sonido Dash")]
    public AudioClip dashSFX;

    [Header("Ajustes de Dash")]
    public float dashForce = 25f; // Ahora actúa como "Velocidad del Dash"
    public float dashDuration = 0.25f;

    [Header("Trail Dash")]
    [SerializeField] private GameObject trailDashObject;

    [Header("Partículas Dash")]
    [SerializeField] private ParticleSystem psIdle;

    [Header("Animación Dash")]
    [SerializeField] private Animator animator;

    [Header("Luz Dash")]
    [SerializeField] private Light luzDash;

    // Variables de respaldo estéticas
    private Color colorLuzNormal;
    private float intensidadNormal;
    private float rangeNormal;

    private Color psColorNormal;
    private float psSpeedNormal;
    private float psSizeNormal;
    private float psRateNormal;

    // NUEVO: Control de estado del dash
    private bool estaDaseando = false;

    void Start()
    {
        if (playerCam == null)
            playerCam = Camera.main.transform;

        // Ya NO buscamos ni inicializamos el Rigidbody
        controller = GetComponent<CharacterController>();

        // Respaldo de partículas
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

    // Propiedad pública para que PlayerMovimiento sepa si estamos en mitad de un dash
    public bool IsDashing => estaDaseando;

    public void ExecuteDash()
    {
        // Evitamos ejecutar un dash si ya estamos en uno
        if (!estaDaseando)
        {
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        estaDaseando = true;
        ActivarTrailDash();
        SFXManager.Instance.PlaySFX(dashSFX, transform, 1f);

        // Conseguimos la dirección exacta del dash basada en los inputs y la cámara
        Vector3 direction = GetDirection();

        // NUEVO: Forzar la rotación instantánea del jugador hacia donde va a dasear
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        float tiempoTranscurrido = 0f;

        // Bucle que moverá al jugador frame a frame de manera cinemática
        while (tiempoTranscurrido < dashDuration)
        {
            // Calculamos el desplazamiento de este frame: Dirección * Velocidad * DeltaTime
            Vector3 desplazamientoDash = direction * (dashForce * Time.deltaTime);

            // Movemos mediante el CharacterController para respetar colisiones
            if (controller != null && controller.enabled)
            {
                controller.Move(desplazamientoDash);
            }

            tiempoTranscurrido += Time.deltaTime;
            yield return null; // Espera al siguiente frame (Update)
        }

        DesactivarTrailDash();
        estaDaseando = false;
    }

    private void ActivarTrailDash()
    {
        if (animator != null) animator.SetTrigger("Dash");
        if (trailDashObject != null) trailDashObject.SetActive(true);

        if (psIdle != null)
        {
            var main = psIdle.main;
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.85f, 0f, 1f));
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
        if (trailDashObject != null) trailDashObject.SetActive(false);

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

        Vector3 adelanteCamara = playerCam.forward;
        Vector3 derechaCamara = playerCam.right;

        adelanteCamara.y = 0f;
        derechaCamara.y = 0f;

        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        if (h == 0 && v == 0)
            return transform.forward;

        return (derechaCamara * h + adelanteCamara * v).normalized;
    }
}
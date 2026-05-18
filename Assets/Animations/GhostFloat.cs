using UnityEngine;

public class GhostFloat : MonoBehaviour
{
    [Header("Flotación")]
    public float floatAmplitude = 0.5f;
    public float floatSpeed = 1.2f;

    [Header("Balanceo")]
    public float swayAmplitude = 4f;
    public float swaySpeed = 1.0f;

    [Header("Elevación en movimiento")]
    public float cantidadElevacion = 0.5f;

    private Vector3 startPos;
    private float timeOffset;
    private Animator animator;

    void Start()
    {
        startPos = transform.localPosition;
        timeOffset = Random.Range(0f, 10f);
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        float t = Time.time + timeOffset;
        float velocidad = animator != null ? animator.GetFloat("Speed") : 0f;

        // Al moverse sube, al detenerse baja
        float objetivoY = startPos.y + (velocidad * cantidadElevacion);

        // Transición suave
        float actualY = transform.localPosition.y;
        float nuevoY = Mathf.Lerp(actualY, objetivoY, Time.deltaTime * 3f);

        // Se añade la ola de flotación encima
        nuevoY += Mathf.Sin(t * floatSpeed) * floatAmplitude;

        transform.localPosition = new Vector3(
            transform.localPosition.x,
            nuevoY,
            transform.localPosition.z
        );

        // Balanceo en eje Z
        float balanceo = Mathf.Sin(t * swaySpeed) * swayAmplitude;
        transform.localRotation = Quaternion.Euler(
            transform.localRotation.eulerAngles.x,
            transform.localRotation.eulerAngles.y,
            balanceo
        );
    }
}
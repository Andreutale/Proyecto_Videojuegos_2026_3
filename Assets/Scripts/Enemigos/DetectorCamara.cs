using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(GeneradorCono))]
public class DetectorCamara : MonoBehaviour
{
    [Header("Conexiones")]
    public Transform jugador;
    public LayerMask capaObstaculos;

    [Header("Interfaz y Alerta")]
    public float tiempoDeteccion = 1.0f;
    public float velocidadEnfoque = 4f;

    [Header("Dimensiones Matemáticas")]
    public float distanciaVision = 8f;
    public float radioBase = 3f;

    [Header("Visualización")]
    public Color colorNormal = new Color(0, 1, 0, 0.3f);
    public Color colorAlerta = new Color(1, 0, 0, 0.5f);

    [Header("Límite de seguimiento")]
    public float maxAnguloSeguimiento = 45f;

    private Quaternion rotacionInicial;
    private GeneradorCono generador; // Referencia al otro script

    private float timerDeteccion = 0f;
    private bool alertaActivada = false;
    private bool jugadorEncontrado = false;
    private Vector3 ultimoPuntoDeteccion;

    private float tiempoPerdida = 0.6f;
    private float timerPerdida = 0f;

    public bool PlayerDetected => jugadorEncontrado;
    public float DetectionTimer => timerDeteccion;

    void Start()
    {
        rotacionInicial = transform.rotation;
        generador = GetComponent<GeneradorCono>();
    }

    void LateUpdate()
    {
        bool detectadoAhora = PuedeVerJugador();

        if (detectadoAhora)
        {
            jugadorEncontrado = true;
            timerPerdida = 0f;
        }
        else if (jugadorEncontrado)
        {
            timerPerdida += Time.deltaTime;

            if (timerPerdida >= tiempoPerdida)
            {
                jugadorEncontrado = false;
            }
        }

        // --- EL CAMBIO ESTÁ AQUÍ ---
        // Le pasamos el color directamente a los vértices, sin tocar el Material
        if (generador != null)
        {
            generador.colorActual = jugadorEncontrado ? colorAlerta : colorNormal;
        }

        if (jugadorEncontrado)
        {
            timerDeteccion += Time.deltaTime;
            ultimoPuntoDeteccion = jugador.position;

            EnfocarJugador();

            if (timerDeteccion >= tiempoDeteccion && !alertaActivada)
            {
                ActivarAlerta();
            }
        }
        else
        {
            timerDeteccion = 0f;
            alertaActivada = false;

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionInicial,
                2f * Time.deltaTime
            );
        }
    }

    bool PuedeVerJugador()
    {
        if (jugador == null) return false;

        Vector3 jugadorLocal = transform.InverseTransformPoint(jugador.position);

        if (jugadorLocal.z < 0 || jugadorLocal.z > distanciaVision)
            return false;

        float radioEnEstaProfundidad = (jugadorLocal.z / distanciaVision) * radioBase;
        float distanciaAlCentro = new Vector2(jugadorLocal.x, jugadorLocal.y).magnitude;

        if (distanciaAlCentro > radioEnEstaProfundidad)
            return false;

        Vector3 direccionAlJugador = jugador.position - transform.position;
        if (Physics.Raycast(transform.position, direccionAlJugador.normalized, out RaycastHit hit, direccionAlJugador.magnitude, capaObstaculos))
        {
            return false;
        }

        return true;
    }

    void EnfocarJugador()
    {
        if (jugador == null) return;

        Vector3 objetivo = jugador.position + Vector3.down * 0.5f;
        Vector3 direccion = objetivo - transform.position;

        if (direccion.sqrMagnitude < 0.001f) return;

        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);

        float diferenciaAngulo = Mathf.DeltaAngle(
            rotacionInicial.eulerAngles.y,
            rotacionObjetivo.eulerAngles.y
        );

        if (Mathf.Abs(diferenciaAngulo) > maxAnguloSeguimiento)
        {
            jugadorEncontrado = false;
            return;
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotacionObjetivo,
            velocidadEnfoque * Time.deltaTime
        );
    }

    void ActivarAlerta()
    {
        alertaActivada = true;
        Debug.Log("La cámara ha activado una alerta global en: " + ultimoPuntoDeteccion);
    }
}
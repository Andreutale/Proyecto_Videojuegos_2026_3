using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MovimientoRutaPatrullero : MonoBehaviour
{
    private enum Estado { Patrullando, Investigando, Persiguiendo, EsperandoYGirando }
    [SerializeField] private Estado estadoActual = Estado.Patrullando;

    [Header("Referencias")]
    public OjosPatrullero ojos;
    public Transform jugador;

    [Header("Ruta de Patrulla")]
    public Transform[] puntosDePatrulla;
    public bool patrullaAleatoria = false;

    public float velocidadPatrulla    = 2f;
    public float tiempoDeEspera       = 1.5f;
    public float velocidadGiro        = 3f;

    [Header("Persecución")]
    public float velocidadPersecucion = 3f;
    public float tiempoDeteccion      = 5f;

    [Header("Investigación por Ruido")]
    public float tiempoInvestigacion  = 2.5f;

    private Transform destinoActual;
    private int   indicePuntoActual     = 0;
    private bool  estaCambiandoDePunto  = false;
    private Rigidbody rb;

    private float timerDeteccion  = 0f;
    private bool  derrotaActivada = false;

    private Coroutine rutinaInvestigacionActual;
    private Coroutine rutinaCambioPuntoActual;
    private Animator  animator;

    private Vector3 puntoAlerta;
    private bool    enBusqueda = false;

    void OnEnable()  { AlertaGlobal.OnAlertaGlobal += RecibirAlerta; }
    void OnDisable() { AlertaGlobal.OnAlertaGlobal -= RecibirAlerta; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();

        if (puntosDePatrulla.Length > 0)
            destinoActual = puntosDePatrulla[0];

        if (ojos == null)
            ojos = GetComponent<OjosPatrullero>();
    }

    void Update()
    {
        if (derrotaActivada)
        {
            DetenerMovimientoHorizontal();
            return;
        }

        bool veAlJugador = (ojos != null && ojos.viendoAlJugador);

        if (veAlJugador)
        {
            // Cancela cualquier rutina activa al ver al jugador
            if (rutinaCambioPuntoActual != null)
            {
                StopCoroutine(rutinaCambioPuntoActual);
                rutinaCambioPuntoActual = null;
                estaCambiandoDePunto    = false;
            }
            if (rutinaInvestigacionActual != null)
            {
                StopCoroutine(rutinaInvestigacionActual);
                rutinaInvestigacionActual = null;
            }

            estadoActual = Estado.Persiguiendo;
            enBusqueda   = false;

            timerDeteccion += Time.deltaTime;
            if (DetectionHUD.Instance != null)
                DetectionHUD.Instance.ReportTimer(this, tiempoDeteccion - timerDeteccion);
            if (timerDeteccion >= tiempoDeteccion)
                timerDeteccion = 0f;
        }
        else if (enBusqueda)
        {
            estadoActual = Estado.Persiguiendo;
        }
        else
        {
            if (estadoActual == Estado.Persiguiendo)
            {
                estadoActual = Estado.Patrullando;
                if (puntosDePatrulla.Length > 0)
                    destinoActual = puntosDePatrulla[indicePuntoActual];
            }

            timerDeteccion = 0f;
            if (DetectionHUD.Instance != null)
                DetectionHUD.Instance.RemoveTimer(this);
        }

        switch (estadoActual)
        {
            case Estado.Patrullando:        MoverHaciaDestino();           break;
            case Estado.Investigando:                                       break;
            case Estado.Persiguiendo:       PerseguirJugador();            break;
            case Estado.EsperandoYGirando:  DetenerMovimientoHorizontal(); break;
        }

        if (animator != null)
        {
            float velocidadHorizontal = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
            animator.SetFloat("velocidad", velocidadHorizontal);
        }
    }

    // -------------------------------------------------- MOVIMIENTO

    private void AplicarVelocidadHacia(Vector3 destino, float velocidad)
    {
        Vector3 posPlana      = new Vector3(rb.position.x, 0, rb.position.z);
        Vector3 destinoPlano  = new Vector3(destino.x, 0, destino.z);
        Vector3 direccion     = (destinoPlano - posPlana).normalized;

        rb.linearVelocity = new Vector3(direccion.x * velocidad, rb.linearVelocity.y, direccion.z * velocidad);
    }

    private void DetenerMovimientoHorizontal()
    {
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }

    // -------------------------------------------------- ESTADOS

    void MoverHaciaDestino()
    {
        if (destinoActual == null || puntosDePatrulla.Length == 0) return;
        if (estaCambiandoDePunto) return;

        Vector3 posPlana     = new Vector3(rb.position.x, 0, rb.position.z);
        Vector3 destinoPlano = new Vector3(destinoActual.position.x, 0, destinoActual.position.z);

        if (Vector3.Distance(posPlana, destinoPlano) > 0.3f)
        {
            AplicarVelocidadHacia(destinoPlano, velocidadPatrulla);
            GirarHacia(destinoPlano);
        }
        else
        {
            DetenerMovimientoHorizontal();
            estaCambiandoDePunto    = true;
            rutinaCambioPuntoActual = StartCoroutine(SecuenciaCambioDePunto());
        }
    }

    IEnumerator SecuenciaCambioDePunto()
    {
        estadoActual = Estado.EsperandoYGirando;
        DetenerMovimientoHorizontal();

        yield return new WaitForSeconds(tiempoDeEspera);

        // Calcular siguiente punto
        if (puntosDePatrulla.Length > 1)
        {
            if (patrullaAleatoria)
            {
                int nuevoIndice = indicePuntoActual;
                while (nuevoIndice == indicePuntoActual)
                    nuevoIndice = Random.Range(0, puntosDePatrulla.Length);
                indicePuntoActual = nuevoIndice;
            }
            else
            {
                indicePuntoActual = (indicePuntoActual + 1) % puntosDePatrulla.Length;
            }
        }

        destinoActual = puntosDePatrulla[indicePuntoActual];

        // Girar hacia el nuevo destino
        Vector3 destinoPlano = new Vector3(destinoActual.position.x, 0, destinoActual.position.z);
        float   angulo       = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(destinoPlano - rb.position));

        while (angulo > 2f)
        {
            Vector3 direccion = destinoPlano - new Vector3(rb.position.x, 0, rb.position.z);

            if (direccion.sqrMagnitude > 0.01f)
            {
                Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, velocidadGiro * Time.deltaTime);
                angulo             = Quaternion.Angle(transform.rotation, rotacionDeseada);
            }
            else
            {
                break;
            }

            yield return null;
        }

        estadoActual            = Estado.Patrullando;
        estaCambiandoDePunto    = false;
        rutinaCambioPuntoActual = null;
    }

    void PerseguirJugador()
    {
        Vector3 destino      = enBusqueda ? puntoAlerta : jugador.position;
        Vector3 posPlana     = new Vector3(rb.position.x, 0, rb.position.z);
        Vector3 destinoPlano = new Vector3(destino.x, 0, destino.z);

        AplicarVelocidadHacia(destino, velocidadPersecucion);
        GirarHacia(destino);

        if (enBusqueda && Vector3.Distance(posPlana, destinoPlano) < 0.5f)
        {
            enBusqueda   = false;
            estadoActual = Estado.Patrullando;

            if (puntosDePatrulla.Length > 0)
                destinoActual = puntosDePatrulla[indicePuntoActual];

            Debug.Log(gameObject.name + " llegó al punto de alerta y vuelve a patrullar.");
        }
    }

    void GirarHacia(Vector3 objetivo)
    {
        Vector3 direccion = objetivo - transform.position;
        direccion.y = 0f;

        if (direccion.magnitude > 0.01f)
        {
            Quaternion rotacion = Quaternion.LookRotation(direccion);
            transform.rotation  = Quaternion.Slerp(transform.rotation, rotacion, velocidadGiro * Time.deltaTime);
        }
    }

    public void ReportarInteraccion(Vector3 posicionInteraccion)
    {
        if (derrotaActivada || estadoActual == Estado.Persiguiendo) return;

        if (rutinaInvestigacionActual != null)
            StopCoroutine(rutinaInvestigacionActual);

        estaCambiandoDePunto      = false;
        rutinaInvestigacionActual = StartCoroutine(IrAInvestigar(posicionInteraccion));
    }

    IEnumerator IrAInvestigar(Vector3 punto)
    {
        estadoActual = Estado.Investigando;
        Vector3 puntoPlano = new Vector3(punto.x, 0, punto.z);

        while (Vector3.Distance(new Vector3(rb.position.x, 0, rb.position.z), puntoPlano) > 0.3f)
        {
            AplicarVelocidadHacia(punto, velocidadPatrulla);
            GirarHacia(punto);
            yield return null;
        }

        DetenerMovimientoHorizontal();
        yield return new WaitForSeconds(tiempoInvestigacion);

        rutinaInvestigacionActual = null;
        estadoActual              = Estado.Patrullando;

        if (puntosDePatrulla.Length > 0)
            destinoActual = puntosDePatrulla[indicePuntoActual];
    }

    void RecibirAlerta(Vector3 punto)
    {
        puntoAlerta          = punto;
        enBusqueda           = true;
        estadoActual         = Estado.Persiguiendo;
        estaCambiandoDePunto = false;

        if (rutinaInvestigacionActual != null)
        {
            StopCoroutine(rutinaInvestigacionActual);
            rutinaInvestigacionActual = null;
        }

        if (rutinaCambioPuntoActual != null)
        {
            StopCoroutine(rutinaCambioPuntoActual);
            rutinaCambioPuntoActual = null;
        }
    }
}
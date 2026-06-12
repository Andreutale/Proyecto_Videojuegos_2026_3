using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
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

    public float velocidadPatrulla = 2f;
    public float tiempoDeEspera = 1.5f;
    public float velocidadGiro = 3f;

    [Header("Persecución")]
    public float velocidadPersecucion = 3f;
    public float tiempoDeteccion = 5f;

    [Header("Investigación por Ruido")]
    public float tiempoInvestigacion = 2.5f;

    private Transform destinoActual;
    private int indicePuntoActual = 0;
    private bool estaCambiandoDePunto = false;
    private NavMeshAgent agente;

    private float timerDeteccion = 0f;
    private bool derrotaActivada = false;

    private Coroutine rutinaInvestigacionActual;
    private Coroutine rutinaCambioPuntoActual;
    private Animator animator;

    private Vector3 puntoAlerta;
    private bool enBusqueda = false;

    void OnEnable() { AlertaGlobal.OnAlertaGlobal += RecibirAlerta; }
    void OnDisable() { AlertaGlobal.OnAlertaGlobal -= RecibirAlerta; }

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidadPatrulla;
        agente.angularSpeed = 120f;
        agente.acceleration = 8f;
        agente.stoppingDistance = 0.3f;

        animator = GetComponent<Animator>();

        if (puntosDePatrulla.Length > 0)
        {
            destinoActual = puntosDePatrulla[0];
            agente.SetDestination(destinoActual.position);
        }

        if (ojos == null)
            ojos = GetComponent<OjosPatrullero>();
    }

    void Update()
    {
        if (derrotaActivada)
        {
            agente.isStopped = true;
            return;
        }

        bool veAlJugador = (ojos != null && ojos.viendoAlJugador);

        if (veAlJugador)
        {
            if (rutinaCambioPuntoActual != null)
            {
                StopCoroutine(rutinaCambioPuntoActual);
                rutinaCambioPuntoActual = null;
                estaCambiandoDePunto = false;
            }
            if (rutinaInvestigacionActual != null)
            {
                StopCoroutine(rutinaInvestigacionActual);
                rutinaInvestigacionActual = null;
            }

            estadoActual = Estado.Persiguiendo;
            enBusqueda = false;

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
                {
                    destinoActual = puntosDePatrulla[indicePuntoActual];
                    agente.SetDestination(destinoActual.position);
                }
            }

            timerDeteccion = 0f;
            if (DetectionHUD.Instance != null)
                DetectionHUD.Instance.RemoveTimer(this);
        }

        switch (estadoActual)
        {
            case Estado.Patrullando: MoverHaciaDestino(); break;
            case Estado.Investigando: break;
            case Estado.Persiguiendo: PerseguirJugador(); break;
            case Estado.EsperandoYGirando: agente.isStopped = true; break;
        }

        if (animator != null)
        {
            float velocidadHorizontal = new Vector3(agente.velocity.x, 0, agente.velocity.z).magnitude;
            animator.SetFloat("velocidad", velocidadHorizontal);
        }
    }

    void MoverHaciaDestino()
    {
        if (destinoActual == null || puntosDePatrulla.Length == 0) return;
        if (estaCambiandoDePunto) return;

        agente.isStopped = false;
        agente.speed = velocidadPatrulla;

        if (!agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
        {
            agente.isStopped = true;
            estaCambiandoDePunto = true;
            rutinaCambioPuntoActual = StartCoroutine(SecuenciaCambioDePunto());
        }
    }

    IEnumerator SecuenciaCambioDePunto()
    {
        estadoActual = Estado.EsperandoYGirando;
        agente.isStopped = true;

        yield return new WaitForSeconds(tiempoDeEspera);

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
        agente.SetDestination(destinoActual.position);
        agente.isStopped = false;

        estadoActual = Estado.Patrullando;
        estaCambiandoDePunto = false;
        rutinaCambioPuntoActual = null;
    }

    void PerseguirJugador()
    {
        Vector3 destino = enBusqueda ? puntoAlerta : jugador.position;
        agente.isStopped = false;
        agente.speed = velocidadPersecucion;
        agente.SetDestination(destino);

        Vector3 posPlana = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 destinoPlano = new Vector3(destino.x, 0, destino.z);

        if (enBusqueda && Vector3.Distance(posPlana, destinoPlano) < 0.5f)
        {
            enBusqueda = false;
            estadoActual = Estado.Patrullando;

            if (puntosDePatrulla.Length > 0)
            {
                destinoActual = puntosDePatrulla[indicePuntoActual];
                agente.SetDestination(destinoActual.position);
            }
        }
    }

    public void ReportarInteraccion(Vector3 posicionInteraccion)
    {
        if (derrotaActivada || estadoActual == Estado.Persiguiendo) return;

        if (rutinaInvestigacionActual != null)
            StopCoroutine(rutinaInvestigacionActual);

        estaCambiandoDePunto = false;
        rutinaInvestigacionActual = StartCoroutine(IrAInvestigar(posicionInteraccion));
    }

    IEnumerator IrAInvestigar(Vector3 punto)
    {
        estadoActual = Estado.Investigando;
        agente.isStopped = false;
        agente.speed = velocidadPatrulla;
        agente.SetDestination(punto);

        while (agente.remainingDistance > 0.3f || agente.pathPending)
            yield return null;

        agente.isStopped = true;
        yield return new WaitForSeconds(tiempoInvestigacion);

        rutinaInvestigacionActual = null;
        estadoActual = Estado.Patrullando;

        if (puntosDePatrulla.Length > 0)
        {
            destinoActual = puntosDePatrulla[indicePuntoActual];
            agente.SetDestination(destinoActual.position);
        }
    }

    void RecibirAlerta(Vector3 punto)
    {
        puntoAlerta = punto;
        enBusqueda = true;
        estadoActual = Estado.Persiguiendo;
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

        agente.isStopped = false;
        agente.speed = velocidadPersecucion;
        agente.SetDestination(puntoAlerta);
    }
}
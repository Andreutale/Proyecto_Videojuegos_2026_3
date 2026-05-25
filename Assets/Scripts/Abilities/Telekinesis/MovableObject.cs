using UnityEngine;
using Possession;
using System.Collections;

namespace Telekinesis
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovableObject : MonoBehaviour
    {
        private Rigidbody rb;
        private Vector3 originalPosition;
        private Coroutine floatCoroutine;

        [Header("Peso")]
        [SerializeField] private WeightClass weightClass = WeightClass.Medium;

        [Header("Multiplicadores por peso")]
        [SerializeField] private float fuerzaLigero = 1.5f;
        [SerializeField] private float fuerzaMedio   = 1.0f;
        [SerializeField] private float fuerzaPesado  = 0.5f;

        [SerializeField] private float ruidoLigero = 0.6f;
        [SerializeField] private float ruidoMedio   = 1.0f;
        [SerializeField] private float ruidoPesado  = 1.8f;

        [Header("Sistema de Sonido (Ruido de Impacto)")]
        [SerializeField] private float umbralImpacto      = 2f;
        [SerializeField] private float multiplicadorRuido = 1.5f;
        [SerializeField] private float radioMaximoSonido  = 30f;

        [Header("Sistema de Sonido (Ruido de Arrastre)")]
        [SerializeField] private float umbralArrastre             = 1f;
        [SerializeField] private float intervaloRuidoArrastre     = 0.4f;
        [SerializeField] private float multiplicadorRuidoArrastre = 0.8f;

        [Header("Friccion por Script")]
        [SerializeField] private float fuerzaFrenado    = 5f;
        [SerializeField] private float umbralParadaTotal = 0.1f;

        private bool  tocandoSuperficie = false;
        private float timerArrastre     = 0f;
        private DirectionArrow arrow;

        public WeightClass WeightClass => weightClass;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            arrow = gameObject.AddComponent<DirectionArrow>();
        }

        
        public void UpdateArrow(Vector3 direction) => arrow.UpdateDirection(direction);
        public void ShowArrow(Vector3 direction)
        {
            arrow.Show(direction);
            originalPosition = transform.position;
            floatCoroutine = StartCoroutine(LevitarObjeto());
        }

        public void HideArrow()
        {
            arrow.Hide();
            if (floatCoroutine != null) StopCoroutine(floatCoroutine);
        }

        public void ApplyForce(Vector3 direction, float force)
        {
            // Parar levitación
            if (floatCoroutine != null) StopCoroutine(floatCoroutine);

            // Bajar el objeto a su posición original antes de lanzar
            transform.position = originalPosition;

            rb.isKinematic = false;
            tocandoSuperficie = false;

            float multiplicador = weightClass switch
            {
                WeightClass.Light => fuerzaLigero,
                WeightClass.Medium => fuerzaMedio,
                WeightClass.Heavy => fuerzaPesado,
                _ => fuerzaMedio
            };

            float fuerzaFinal = force * multiplicador;
            rb.AddForce(direction.normalized * fuerzaFinal, ForceMode.Impulse);

            Vector3 launchDirection =
    (direction * 0.7f + Vector3.up * 0.8f).normalized;

            rb.AddForce(launchDirection * force, ForceMode.Impulse);

            // Efecto visual de lanzamiento: giro brusco
            rb.AddTorque(Random.insideUnitSphere * fuerzaFinal * 0.3f, ForceMode.Impulse);

            Debug.Log($"[Telekinesis] Fuerza aplicada a {gameObject.name} | Peso: {weightClass} | Fuerza: {fuerzaFinal}");
        }

        private float GetMultiplicadorRuido()
        {
            return weightClass switch
            {
                WeightClass.Light  => ruidoLigero,
                WeightClass.Medium => ruidoMedio,
                WeightClass.Heavy  => ruidoPesado,
                _                  => ruidoMedio
            };
        }

        private void FixedUpdate()
        {
            if (tocandoSuperficie && !rb.isKinematic)
            {
                Vector3 velocidadHorizontal = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

                velocidadHorizontal    = Vector3.Lerp(velocidadHorizontal, Vector3.zero, Time.fixedDeltaTime * fuerzaFrenado);
                rb.angularVelocity     = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.fixedDeltaTime * fuerzaFrenado);
                rb.linearVelocity      = new Vector3(velocidadHorizontal.x, rb.linearVelocity.y, velocidadHorizontal.z);

                if (velocidadHorizontal.magnitude < umbralParadaTotal)
                {
                    rb.linearVelocity  = new Vector3(0, rb.linearVelocity.y, 0);
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            tocandoSuperficie = true;

            float velocidadImpacto = collision.relativeVelocity.magnitude;

            if (velocidadImpacto >= umbralImpacto)
            {
                Vector3 puntoDeImpacto = collision.contacts[0].point;
                GenerarOndaDeSonido(velocidadImpacto, puntoDeImpacto, multiplicadorRuido * GetMultiplicadorRuido());
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            tocandoSuperficie = true;

            float velocidadActual = rb.linearVelocity.magnitude;

            if (velocidadActual >= umbralArrastre)
            {
                timerArrastre += Time.deltaTime;

                if (timerArrastre >= intervaloRuidoArrastre)
                {
                    timerArrastre = 0f;
                    Vector3 puntoDeFriccion = collision.contacts[0].point;
                    GenerarOndaDeSonido(velocidadActual, puntoDeFriccion, multiplicadorRuidoArrastre * GetMultiplicadorRuido());
                }
            }
            else
            {
                timerArrastre = 0f;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            tocandoSuperficie = false;
            timerArrastre     = 0f;
        }

        private void GenerarOndaDeSonido(float velocidad, Vector3 origen, float multiplicador)
        {
            float fuerzaGolpe    = rb.mass * velocidad;
            float radioCalculado = fuerzaGolpe * multiplicador;
            float radioFinal     = Mathf.Clamp(radioCalculado, 0f, radioMaximoSonido);

            GameObject ondaVisual = new GameObject("OndaSonidoVisual");
            ondaVisual.transform.position = new Vector3(origen.x, origen.y + 0.1f, origen.z);

            EfectoOnda efecto = ondaVisual.AddComponent<EfectoOnda>();
            Color grisOscuro  = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            efecto.Iniciar(radioFinal, grisOscuro, 1.5f);

            MovimientoRutaPatrullero[] enemigos = FindObjectsByType<MovimientoRutaPatrullero>(FindObjectsSortMode.None);
            foreach (MovimientoRutaPatrullero enemigo in enemigos)
            {
                float distanciaAlRuido = Vector3.Distance(origen, enemigo.transform.position);
                if (distanciaAlRuido <= radioFinal)
                    enemigo.ReportarInteraccion(origen);
            }
        }
        private IEnumerator LevitarObjeto()
        {
            // — Fase 1: subir el objeto (0.3s) —
            float subirDuration = 0.3f;
            float elapsed = 0f;
            Vector3 targetPos = originalPosition + Vector3.up * 0.6f;

            rb.isKinematic = true; // congelamos físicas mientras levita

            while (elapsed < subirDuration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(originalPosition, targetPos, elapsed / subirDuration);
                yield return null;
            }

            // — Fase 2: flotar arriba y abajo indefinidamente —
            // — Fase 2: flotar arriba y abajo indefinidamente —
            float t = 0f;
            while (true)
            {
                t += Time.deltaTime;
                float oscilacion = Mathf.Sin(t * 2.5f) * 0.08f;
                transform.position = targetPos + Vector3.up * oscilacion;

                yield return null;
            }

        }
    }

}
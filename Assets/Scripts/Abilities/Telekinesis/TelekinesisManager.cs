using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Telekinesis
{
    public class TelekinesisManager : MonoBehaviour
    {
        [SerializeField] private TelekinesisConfig config;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Camara camara;
        [SerializeField] private TelekinesisOutlineController outlineController;
        [SerializeField] private PlayerMovimiento playerMovimiento;
        [SerializeField] private GhostTelekinesisAnimation ghostAnimation;

        [Header("Sistema de Cooldown UI")]
        [SerializeField] private HabilidadCooldown uiCooldown;

        private TelekinesisInputHandler inputHandler;
        private TelekinesisState currentState = TelekinesisState.Idle;
        private MovableObject currentTarget;
        private List<MovableObject> nearbyObjects = new List<MovableObject>();
        private float scanRefreshTimer;
        private Transform originalCameraTarget;
        private Transform brazoIzq;
        private Transform brazoDer;
        private Coroutine brazoCoroutine;

        // -------------------------------------------------- Unity

        private void Awake()
        {
            inputHandler = GetComponent<TelekinesisInputHandler>();
            inputHandler.OnActionKeyPressed += HandleActionInput;
            inputHandler.OnCancelKeyPressed += Cancel;
            brazoIzq = playerTransform.GetComponentsInChildren<Transform>()[0]; // lo buscamos por nombre abajo
            brazoDer = playerTransform.GetComponentsInChildren<Transform>()[0];

            // Buscar por nombre exacto
            foreach (Transform t in playerTransform.GetComponentsInChildren<Transform>())
            {
                if (t.name == "Line001") brazoIzq = t;
                if (t.name == "Line002") brazoDer = t;
            }
        }

        private void OnDestroy()
        {
            inputHandler.OnActionKeyPressed -= HandleActionInput;
            inputHandler.OnCancelKeyPressed -= Cancel;
        }

        private void Update()
        {
            // 🔹 ACTUALIZAR FLECHA EN TIEMPO REAL (del Script 2)
            if (currentState == TelekinesisState.Aiming && currentTarget != null)
            {
                Vector3 direction = GetWorldDirection(inputHandler.LastDirection);
                currentTarget.UpdateArrow(direction);
            }

            // 🔹 ESCANEO (de ambos scripts)
            if (currentState != TelekinesisState.Scanning) return;

            scanRefreshTimer += Time.deltaTime;
            if (scanRefreshTimer >= 0.1f)
            {
                scanRefreshTimer = 0f;
                List<MovableObject> newNearby = FindAllNearby();

                bool listChanged = newNearby.Count != nearbyObjects.Count;
                nearbyObjects = newNearby;

                if (nearbyObjects.Count == 0)
                {
                    outlineController.HideOutlines();
                    currentTarget = null;
                    return;
                }

                if (listChanged)
                {
                    currentTarget = FindNearestFrom(nearbyObjects);
                    outlineController.ShowOutlines(nearbyObjects, currentTarget);
                    return;
                }
            }

            if (nearbyObjects.Count == 0) return;

            MovableObject nearest = FindNearestFrom(nearbyObjects);
            if (nearest == currentTarget) return;

            currentTarget = nearest;
            outlineController.ShowOutlines(nearbyObjects, currentTarget);
        }

        // -------------------------------------------------- Input

        private void HandleActionInput()
        {
            // 🔹 BLOQUEO POR COOLDOWN
            if (uiCooldown != null && uiCooldown.EstaEnEnfriamiento)
            {
                Debug.Log("[Telekinesis] En cooldown.");
                return;
            }

            // 🔹 BLOQUEO POR OTRA HABILIDAD
            if (!AbilityManager.Instance.CanUseAbility(this)) return;

            switch (currentState)
            {
                case TelekinesisState.Idle:
                    EnterScanning();
                    break;

                case TelekinesisState.Scanning:
                    EnterAiming();
                    break;

                case TelekinesisState.Aiming:
                    ApplyForce();
                    break;
            }
        }

        // -------------------------------------------------- Estados

        private void EnterScanning()
        {
            AbilityManager.Instance.RegisterAbility(this);

            nearbyObjects = FindAllNearby();
            currentTarget = FindNearestFrom(nearbyObjects);
            currentState = TelekinesisState.Scanning;

            if (nearbyObjects.Count > 0)
                outlineController.ShowOutlines(nearbyObjects, currentTarget);

            if (uiCooldown != null)
                uiCooldown.EstablecerUsoActivo(true);

            Debug.Log("[Telekinesis] Escaneando.");
        }

        private void EnterAiming()
        {
            if (currentTarget == null) return;

            currentState = TelekinesisState.Aiming;
            outlineController.HideOutlines();

            playerMovimiento.enabled = false;

            originalCameraTarget = playerTransform;
            camara.SetTarget(currentTarget.transform);

            // 🔹 Mostrar flecha (Script 2)
            Vector3 initialDirection = GetWorldDirection(inputHandler.LastDirection);
            currentTarget.ShowArrow(initialDirection);

            Debug.Log($"[Telekinesis] Apuntando a: {currentTarget.gameObject.name}");
            if (brazoCoroutine != null) StopCoroutine(brazoCoroutine);
            brazoCoroutine = StartCoroutine(EstirarBrazos());
        }

        private void ApplyForce()
        {
            if (currentTarget == null) return;

            Vector3 direction = GetWorldDirection(inputHandler.LastDirection);

            // 🔹 ocultar flecha
            currentTarget.HideArrow();

            currentTarget.ApplyForce(direction, config.pushForce);
            Debug.Log("Intentando lanzar animación fantasma");

            if (ghostAnimation != null)
            {
                ghostAnimation.Play();
            }
            else
            {
                Debug.LogWarning("GhostAnimation NO está asignado en TelekinesisManager");
            }
            // 🔹 activar cooldown
            if (uiCooldown != null)
                uiCooldown.IniciarCooldown();

            Cancel();
        }

        private void Cancel()
        {
            if (uiCooldown != null)
                uiCooldown.EstablecerUsoActivo(false);

            AbilityManager.Instance.ClearAbility(this);

            if (currentState == TelekinesisState.Idle) return;

            if (currentTarget != null)
                currentTarget.HideArrow();

            outlineController.HideOutlines();
            camara.SetTarget(originalCameraTarget);

            playerMovimiento.enabled = true;

            currentTarget = null;
            currentState = TelekinesisState.Idle;

            Debug.Log("[Telekinesis] Cancelada.");
            if (brazoCoroutine != null) StopCoroutine(brazoCoroutine);
            StartCoroutine(RecuperarBrazos());
        }

        // -------------------------------------------------- Dirección (versión mejorada combinada)

        private Vector3 GetWorldDirection(Vector3 inputDir)
        {
            Transform cam = Camera.main.transform;
        
            // Direcciones de la cámara en plano horizontal
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
        
            camForward.y = 0;
            camRight.y = 0;
        
            camForward.Normalize();
            camRight.Normalize();
        
            // Convertimos input a mundo RELATIVO A LA CÁMARA
            Vector3 direction = camForward * inputDir.z + camRight * inputDir.x;
        
            // Si no hay input → hacia delante de la cámara
            if (direction.sqrMagnitude < 0.01f)
                direction = camForward;
        
            return direction.normalized;
        }

        // -------------------------------------------------- Detección

        private List<MovableObject> FindAllNearby()
        {
            Collider[] hits = Physics.OverlapSphere(
                playerTransform.position,
                config.detectionRadius
            );

            List<MovableObject> result = new List<MovableObject>();

            foreach (Collider hit in hits)
            {
                if (hit.TryGetComponent(out MovableObject obj))
                    result.Add(obj);
            }

            return result;
        }

        private MovableObject FindNearestFrom(List<MovableObject> objects)
        {
            MovableObject nearest = null;
            float bestDist = float.MaxValue;

            foreach (MovableObject obj in objects)
            {
                float dist = Vector3.Distance(playerTransform.position, obj.transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    nearest = obj;
                }
            }

            return nearest;
        }

        private IEnumerator EstirarBrazos()
        {
            if (brazoIzq == null || brazoDer == null) yield break;

            Vector3 scaleOriginalIzq = brazoIzq.localScale;
            Vector3 scaleOriginalDer = brazoDer.localScale;

            float duration = 0.3f;
            float elapsed = 0f;

            // Guardar escalas originales para poder recuperarlas
            brazoIzq.GetComponent<MonoBehaviour>(); // solo para no perder la referencia

            Vector3 scaleEstiradaIzq = new Vector3(scaleOriginalIzq.x, scaleOriginalIzq.y, scaleOriginalIzq.z * 2.5f);
            Vector3 scaleEstiradaDer = new Vector3(scaleOriginalDer.x, scaleOriginalDer.y, scaleOriginalDer.z * 2.5f);

            // Fase 1: estirar hacia el objeto (0.3s)
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                brazoIzq.localScale = Vector3.Lerp(scaleOriginalIzq, scaleEstiradaIzq, t);
                brazoDer.localScale = Vector3.Lerp(scaleOriginalDer, scaleEstiradaDer, t);

                // Orientar brazos hacia el objeto
                if (currentTarget != null)
                {
                    Vector3 dir = (currentTarget.transform.position - playerTransform.position).normalized;
                    brazoIzq.rotation = Quaternion.LookRotation(dir);
                    brazoDer.rotation = Quaternion.LookRotation(dir);
                }

                yield return null;
            }

            // Fase 2: mantener estirados y vibrar de esfuerzo
            while (currentState == TelekinesisState.Aiming)
            {
                float vibra = Mathf.Sin(Time.time * 20f) * 0.03f;

                brazoIzq.localScale = scaleEstiradaIzq + Vector3.one * vibra;
                brazoDer.localScale = scaleEstiradaDer + Vector3.one * vibra;

                // Seguir apuntando al objeto en tiempo real
                if (currentTarget != null)
                {
                    Vector3 dir = (currentTarget.transform.position - playerTransform.position).normalized;
                    brazoIzq.rotation = Quaternion.LookRotation(dir);
                    brazoDer.rotation = Quaternion.LookRotation(dir);
                }

                yield return null;
            }
        }

        private IEnumerator RecuperarBrazos()
        {
            if (brazoIzq == null || brazoDer == null) yield break;

            Vector3 scaleActualIzq = brazoIzq.localScale;
            Vector3 scaleActualDer = brazoDer.localScale;
            Quaternion rotActualIzq = brazoIzq.rotation;
            Quaternion rotActualDer = brazoDer.rotation;

            // Escala y rotación originales — ajusta estos valores a los de tu modelo
            Vector3 scaleOriginal = new Vector3(1f, 1f, 1f);
            Quaternion rotOriginal = Quaternion.identity;

            float duration = 0.2f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                brazoIzq.localScale = Vector3.Lerp(scaleActualIzq, scaleOriginal, t);
                brazoDer.localScale = Vector3.Lerp(scaleActualDer, scaleOriginal, t);
                brazoIzq.rotation = Quaternion.Lerp(rotActualIzq, rotOriginal, t);
                brazoDer.rotation = Quaternion.Lerp(rotActualDer, rotOriginal, t);

                yield return null;
            }
        }
    }
}
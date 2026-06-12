using UnityEngine;

namespace Possession
{
    [RequireComponent(typeof(Rigidbody))]
    public class PossessionMovement : MonoBehaviour
    {
        private Rigidbody rb;
        private float currentSpeed;
        private bool isActive;
        private Transform cam;

        [Header("Liberar objetos encima")]
        [SerializeField] private float radioDespertar = 1.5f;

        public bool EstaActivo => isActive;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            cam = Camera.main.transform;
        }

        private void FixedUpdate()
        {
            if (!isActive) return;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 camForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;

            Vector3 direction = (camForward * v + camRight * h).normalized;
            Vector3 velocity = direction * currentSpeed;

            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

            LiberarObjetosEncima();
        }

        private void LiberarObjetosEncima()
        {
            Collider[] cercanos = Physics.OverlapSphere(rb.position, radioDespertar);
            foreach (Collider col in cercanos)
            {
                if (col.attachedRigidbody == null || col.attachedRigidbody == rb)
                    continue;

                PossessableObject possessable = col.GetComponent<PossessableObject>();
                if (possessable == null)
                    continue;

                PossessionMovement otroMovement = col.GetComponent<PossessionMovement>();
                if (otroMovement != null && !otroMovement.EstaActivo)
                {
                    col.attachedRigidbody.isKinematic = false;
                }
            }
        }

        public void Activate(float speed)
        {
            currentSpeed = speed;
            isActive = true;
            rb.isKinematic = false;
        }

        public void Deactivate()
        {
            isActive = false;
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}
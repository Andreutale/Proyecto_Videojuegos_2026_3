using UnityEngine;

public class CameraIdleRotation : MonoBehaviour
{
    public Transform visionCone;
    public DetectorCamara detector;

    [Header("Camara hija que mira arriba/abajo")]
    public Transform cameraChild;

    [Header("Idle horizontal")]
    public float idleSpeed = 1f;
    public float maxAngle = 25f;
    public float returnSpeed = 2f;

    [Header("Movimiento vertical")]
    public float verticalAngle = 12f;
    public float verticalSpeedMultiplier = 0.6f;

    private Quaternion startRotation;
    private Quaternion cameraStartLocalRotation;

    void Start()
    {
        startRotation = transform.rotation;

        if (cameraChild != null)
            cameraStartLocalRotation = cameraChild.localRotation;
    }

    void LateUpdate()
    {
        if (detector != null && detector.PlayerDetected && visionCone != null)
        {
            Vector3 coneEuler = visionCone.rotation.eulerAngles;

            Quaternion targetRotation = Quaternion.Euler(
                startRotation.eulerAngles.x,
                coneEuler.y,
                startRotation.eulerAngles.z
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                returnSpeed * Time.deltaTime
            );

            return;
        }

        float horizontal = Mathf.Sin(Time.time * idleSpeed) * maxAngle;
        float vertical = Mathf.Sin(Time.time * idleSpeed * verticalSpeedMultiplier) * verticalAngle;

        transform.rotation =
            startRotation * Quaternion.Euler(0f, horizontal, 0f);

        if (cameraChild != null)
        {
            cameraChild.localRotation =
                cameraStartLocalRotation * Quaternion.Euler(vertical, 0f, 0f);
        }
    }
}
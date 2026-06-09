using UnityEngine;

public class KeyAnimation : MonoBehaviour
{
    public float rotationSpeed = 45f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.1f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Cambia el eje aquí
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);

        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = new Vector3(
            startPos.x,
            newY,
            startPos.z
        );
    }
}
using UnityEngine;

public class KeyAnimation : MonoBehaviour
{
    public float rotationSpeed = 80f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.15f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Cambia el eje aquí
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = new Vector3(
            startPos.x,
            newY,
            startPos.z
        );
    }
}
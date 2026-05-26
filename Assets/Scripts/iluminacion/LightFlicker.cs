using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light[] luces;

    public float minIntensity = 3f;
    public float maxIntensity = 4f;

    void Update()
    {
        float intensidad = Random.Range(minIntensity, maxIntensity);

        foreach (Light luz in luces)
        {
            if (luz != null)
            {
                luz.intensity = intensidad;
            }
        }
    }
}
using UnityEngine;
using System.Collections;

public class GestorEstrellas : MonoBehaviour
{
    [Header("Objetos Estrella (Izquierda → Derecha)")]
    public AnimacionEstrella[] estrellas = new AnimacionEstrella[3];

    [Tooltip("Tiempo entre cada estrella (segundos)")]
    public float intervalo = 0.2f;

    [Tooltip("Espera antes de la primera estrella")]
    public float retrasoInicial = 0.1f;

    private void OnEnable()
    {
        foreach (var e in estrellas)
            if (e != null)
                e.gameObject.SetActive(false);
    }

    public void MostrarEstrellas(int cantidad)
    {
        cantidad = Mathf.Clamp(cantidad, 0, estrellas.Length);
        StopAllCoroutines();
        StartCoroutine(SecuenciaEstrellas(cantidad));
    }

    private IEnumerator SecuenciaEstrellas(int cantidad)
    {
        yield return new WaitForSecondsRealtime(retrasoInicial);

        for (int i = 0; i < cantidad; i++)
        {
            if (estrellas[i] != null)
                estrellas[i].Activar();

            if (i < cantidad - 1)
                yield return new WaitForSecondsRealtime(intervalo);
        }
    }

    [ContextMenu("Probar 1 Estrella")] void P1() => MostrarEstrellas(1);
    [ContextMenu("Probar 2 Estrellas")] void P2() => MostrarEstrellas(2);
    [ContextMenu("Probar 3 Estrellas")] void P3() => MostrarEstrellas(3);
}
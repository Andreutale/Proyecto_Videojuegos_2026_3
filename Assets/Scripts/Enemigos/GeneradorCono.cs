using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GeneradorCono : MonoBehaviour
{
    [Header("Ajustes de Visión (Reales)")]
    public float distanciaVision = 8f;
    public float radioBase = 3f;
    [Range(16, 128)] public int resolucion = 64;

    [Header("Colisión de Malla")]
    public LayerMask capaObstaculos;

    // Esta variable ahora la controla el DetectorCamara
    [HideInInspector] public Color colorActual = new Color(0, 1, 0, 0.3f);

    Mesh mesh;
    MeshFilter meshFilter;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.name = "ConoCamara_Mesh_Sólido";
        meshFilter.mesh = mesh;
    }

    void LateUpdate()
    {
        GenerarMallaConoSolido();
    }

    void GenerarMallaConoSolido()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangulos = new List<int>();
        List<Color> colores = new List<Color>();

        // El color final es el mismo que el actual, pero con Alfa a 0 (invisible)
        Color colorFin = new Color(colorActual.r, colorActual.g, colorActual.b, 0f);

        // 0. Vértice central (origen de la cámara)
        vertices.Add(Vector3.zero);
        colores.Add(colorActual);

        // Generar vértices del borde del cono
        for (int i = 0; i < resolucion; i++)
        {
            float progreso = (float)i / resolucion;
            float anguloRad = progreso * Mathf.PI * 2;

            float x = Mathf.Cos(anguloRad) * radioBase;
            float y = Mathf.Sin(anguloRad) * radioBase;

            Vector3 puntoBaseIdeal = new Vector3(x, y, distanciaVision);
            Vector3 dirGlobal = transform.TransformDirection(puntoBaseIdeal.normalized);
            int mask = capaObstaculos;

            float diagonalLength = Mathf.Sqrt(radioBase * radioBase + distanciaVision * distanciaVision);

            if (Physics.Raycast(transform.position, dirGlobal, out RaycastHit hit, diagonalLength, mask, QueryTriggerInteraction.Ignore))
            {
                Vector3 puntoLocalHit = transform.InverseTransformPoint(hit.point);
                vertices.Add(puntoLocalHit);

                float porcentajeDistancia = Mathf.Clamp01(puntoLocalHit.z / distanciaVision);
                colores.Add(Color.Lerp(colorActual, colorFin, porcentajeDistancia));
            }
            else
            {
                vertices.Add(puntoBaseIdeal);
                colores.Add(colorFin);
            }
        }

        // --- CORRECCIÓN APLICADA AQUÍ ---
        int indiceCentroTapa = vertices.Count;

        // Definimos dónde debería ir el centro idealmente
        Vector3 centroTapaIdeal = new Vector3(0, 0, distanciaVision);
        Vector3 centroTapaReal = centroTapaIdeal;
        Color colorCentroTapa = colorFin;

        // Lanzamos un rayo hacia adelante (centro del cono) para ver si la pared lo frena
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitCentro, distanciaVision, capaObstaculos, QueryTriggerInteraction.Ignore))
        {
            // Si choca, acortamos el punto central hasta la pared
            centroTapaReal = transform.InverseTransformPoint(hitCentro.point);

            // Calculamos el color adecuado según la distancia a la que se ha quedado
            float porcentajeDistanciaCentro = Mathf.Clamp01(centroTapaReal.z / distanciaVision);
            colorCentroTapa = Color.Lerp(colorActual, colorFin, porcentajeDistanciaCentro);
        }

        // Añadimos el vértice corregido
        vertices.Add(centroTapaReal);
        colores.Add(colorCentroTapa);
        // --------------------------------

        // Generar triángulos de las paredes laterales del cono
        for (int i = 1; i <= resolucion; i++)
        {
            int siguiente = (i == resolucion) ? 1 : i + 1;
            triangulos.Add(0);
            triangulos.Add(siguiente);
            triangulos.Add(i);
        }

        // Generar triángulos de la tapa frontal (base del cono)
        for (int i = 1; i <= resolucion; i++)
        {
            int siguiente = (i == resolucion) ? 1 : i + 1;
            triangulos.Add(indiceCentroTapa);
            triangulos.Add(i);
            triangulos.Add(siguiente);
        }

        // Actualizar la malla
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangulos.ToArray();
        mesh.colors = colores.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
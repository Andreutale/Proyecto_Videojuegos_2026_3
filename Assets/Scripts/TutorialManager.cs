using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public GameObject panelTutorial;
    public TMP_Text textoTutorial;

    public void MostrarMensaje(string mensaje, float tiempo)
    {
        StartCoroutine(Mostrar(mensaje, tiempo));
    }

    IEnumerator Mostrar(string mensaje, float tiempo)
    {
        panelTutorial.SetActive(true);
        textoTutorial.text = mensaje;

        yield return new WaitForSeconds(tiempo);

        panelTutorial.SetActive(false);
    }
}
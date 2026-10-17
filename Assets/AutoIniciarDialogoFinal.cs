using System.Collections;
using UnityEngine;

public class AutoIniciarDialogoFinal : MonoBehaviour
{
    [Header("Referencias")]
    public DialogoLog sistemaDialogo;

    [Header("Configuración")]
    public float retardoInicial = 0.5f;

    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(retardoInicial);

        if (sistemaDialogo != null)
        {
            // Usamos el método correcto de tu script DialogoLog
            sistemaDialogo.EmpezarDialog();
        }
        else
        {
            Debug.LogWarning("AutoIniciarDialogoFinal: No se asignó el sistemaDialogo en el Inspector.");
        }
    }
}
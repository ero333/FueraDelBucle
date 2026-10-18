using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ControladorVideoSimulado : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tiempo en segundos que durará la escena simulando el video")]
    public float tiempoDeEspera = 5f;

    [Tooltip("Nombre de la escena a la que pasará después del tiempo")]
    public string escenaSiguiente;

    void Start()
    {
        // Inicia la cuenta regresiva en cuanto carga esta escena
        StartCoroutine(EsperarYCambiar());
    }

    IEnumerator EsperarYCambiar()
    {
        // Espera la cantidad de segundos indicada
        yield return new WaitForSeconds(tiempoDeEspera);

        // Carga la siguiente escena
        SceneManager.LoadScene(escenaSiguiente);
    }
}
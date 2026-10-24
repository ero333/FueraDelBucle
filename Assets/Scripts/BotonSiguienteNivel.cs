using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonSiguienteNivel : MonoBehaviour
{
    [Header("Nombre de la Escena Destino")]
    [Tooltip("Escribe aquí el nombre exacto de la escena a la que debe ir el botón.")]
    public string nombreEscenaSiguiente = "Final";

    /// <summary>
    /// Asigna esta función pública al evento OnClick() del botón en el Inspector.
    /// </summary>
    public void CargarSiguienteEscena()
    {
        if (!string.IsNullOrEmpty(nombreEscenaSiguiente))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nombreEscenaSiguiente);
        }
        else
        {
            Debug.LogError("BotonSiguienteNivel: No se especificó el nombre de la escena.");
        }
    }
}
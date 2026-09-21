using UnityEngine;
using UnityEngine.SceneManagement;

public class CompletarNivel : MonoBehaviour
{
    [Header("Configuración de Nivel")]
    [Tooltip("Debe coincidir exactamente con la 'Clave Nivel' del menú.")]
    public string claveNivel = "Nivel1";

    [Header("Opciones de Navegación")]
    public string nombreEscenaMenu = "MapaNiveles";

    // Si usas un Trigger (una zona transparente que el jugador toca al llegar a la meta)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GanarNivel();
        }
    }

    // Llama a esta función cuando el jugador gane (por Trigger, botón o evento)
    public void GanarNivel()
    {
        // 1. Guardar el progreso
        PlayerPrefs.SetInt(claveNivel, 1);
        PlayerPrefs.Save();

        Debug.Log("¡Progreso guardado con éxito! Clave: " + claveNivel);

        // 2. Opcional: Cargar de vuelta la escena del mapa de niveles
        if (!string.IsNullOrEmpty(nombreEscenaMenu))
        {
            SceneManager.LoadScene(nombreEscenaMenu);
        }
    }
}
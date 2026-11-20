using UnityEngine;
using UnityEngine.SceneManagement;

public class CompletarNivel : MonoBehaviour
{
    [Header("Configuración de Nivel")]
    [Tooltip("Debe coincidir exactamente con la 'Clave Nivel' del menú.")]
    public string claveNivel = "Nivel1";

    [Header("Opciones de Navegación")]
    public string nombreEscenaMenu = "MapaNiveles";

    private bool yaGano = false;

    // Enter + Stay: si el jugador ya está dentro del trigger cuando se habilita
    // o cuando se cierra un diálogo, igual lo detecta sin tener que salir y volver a entrar.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Intentar(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Intentar(collision);
    }

    private void Intentar(Collider2D collision)
    {
        if (yaGano) return;
        if (!collision.CompareTag("Player")) return;

        yaGano = true;
        GanarNivel();
    }

    // Llama a esta función cuando el jugador gane (por Trigger, botón o evento)
    public void GanarNivel()
    {
        // 1. Guardar el progreso
        PlayerPrefs.SetInt(claveNivel, 1);
        PlayerPrefs.Save();

        Debug.Log("¡Progreso guardado con éxito! Clave: " + claveNivel);

        // 2. Cargar de vuelta la escena del mapa de niveles
        if (!string.IsNullOrEmpty(nombreEscenaMenu))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nombreEscenaMenu);
        }
    }
}
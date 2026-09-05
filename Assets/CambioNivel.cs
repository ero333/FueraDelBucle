using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CambioNivel : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("Nombre de la escena a la que se cambiará.")]
    public string nombreEscenaSiguiente;

    [Tooltip("Tag que debe tener el jugador para activar la interacción.")]
    public string tagJugador = "Player";

    [Header("Interfaz de Usuario")]
    [Tooltip("Arrastra aquí el Botón de la UI desde la jerarquía.")]
    public Button botonCambioEscena;

    private void Start()
    {
        
        if (botonCambioEscena != null)
        {
            botonCambioEscena.gameObject.SetActive(false);

            
            botonCambioEscena.onClick.AddListener(CargarSiguienteEscena);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el botón en el Inspector de CambioNivel.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.CompareTag(tagJugador) && botonCambioEscena != null)
        {
            botonCambioEscena.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
       
        if (other.CompareTag(tagJugador) && botonCambioEscena != null)
        {
            botonCambioEscena.gameObject.SetActive(false);
        }
    }

    
    public void CargarSiguienteEscena()
    {
        SceneManager.LoadScene(nombreEscenaSiguiente);
    }
}

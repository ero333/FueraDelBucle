using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class BotonFinal : MonoBehaviour
{
    [Header("Escena")]
    public string nombreEscenaSiguiente = "Menu";

    [Header("Jugador")]
    public MonoBehaviour controlJugador;

    private bool yaInteractuo = false;

    private void OnMouseDown()
    {
        if (!yaInteractuo)
        {
            yaInteractuo = true;

            if (controlJugador != null)
            {
                controlJugador.enabled = false;
            }

            
            CargarEscenaSiguiente();
        }
    }

    public void CargarEscenaSiguiente()
    {
        if (!string.IsNullOrEmpty(nombreEscenaSiguiente))
        {
            SceneManager.LoadScene(nombreEscenaSiguiente);
        }
        else
        {
            Debug.LogError("No especificaste el nombre de la escena en el Inspector.");
        }
    }
}
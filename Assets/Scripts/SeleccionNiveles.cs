using UnityEngine;
using UnityEngine.SceneManagement;

public class SeleccionNiveles : MonoBehaviour
{
    public void CambiarNivel(string nombreNivel)
    {
        SceneManager.LoadScene(nombreNivel);
    }

    public void CambiarNivel(int numeroNivel)
    {
        SceneManager.LoadScene(numeroNivel);
    }

    // Volver desde el mapa
    public void VolverDesdeMapa()
    {
        bool desdeMenu = PlayerPrefs.GetInt("MapaDesdeMenu", 1) == 1;

        if (desdeMenu)
        {
            SceneManager.LoadScene("MenuPrincipal");
        }
        else
        {
            string ultimoNivel = PlayerPrefs.GetString("UltimoNivelJugado", "Nivel1");
            SceneManager.LoadScene(ultimoNivel);
        }
    }
}

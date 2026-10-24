using UnityEngine;
using UnityEngine.SceneManagement;

public class SeleccionNiveles : MonoBehaviour
{
    public void CambiarNivel(string nombreNivel)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreNivel);
    }

    public void CambiarNivel(int numeroNivel)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(numeroNivel);
    }

    // Volver desde el mapa
    public void VolverDesdeMapa()
    {
        bool desdeMenu = PlayerPrefs.GetInt("MapaDesdeMenu", 1) == 1;

        if (desdeMenu)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MenuPrincipal");
        }
        else
        {
            string ultimoNivel = PlayerPrefs.GetString("UltimoNivelJugado", "Nivel1");
            Time.timeScale = 1f;
            SceneManager.LoadScene(ultimoNivel);
        }
    }
}

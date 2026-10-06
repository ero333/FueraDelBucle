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

    public void VolverAlUltimoNivel()
    {
        // Lee la memoria de Unity y vuelve a la escena guardada
        string ultimoNivel = PlayerPrefs.GetString("UltimoNivelJugado", "MenuPrincipal");
        SceneManager.LoadScene(ultimoNivel);
    }
}

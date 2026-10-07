using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private string escenaJuego = "MapaNiveles";

    // BOTÓN JUGAR / LEVELS
    public void Jugar()
    {
        PlayerPrefs.SetInt("MapaDesdeMenu", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(escenaJuego);
    }

}
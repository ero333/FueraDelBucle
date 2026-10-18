using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private string escenaIntro = "Intro";
    [SerializeField] private string escenaMapa = "MapaNiveles";

    // BOTÓN JUGAR
    public void Jugar()
    {
        PlayerPrefs.SetInt("MapaDesdeMenu", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(escenaIntro);
    }

    // BOTON LEVELS: va directo al mapa de niveles, sin la animacion de intro
    public void Niveles()
    {
        PlayerPrefs.SetInt("MapaDesdeMenu", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(escenaMapa);
    }
}
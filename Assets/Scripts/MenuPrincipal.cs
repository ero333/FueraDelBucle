using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private string escenaIntro = "Intro";

    // BOTÓN JUGAR
    public void Jugar()
    {
        PlayerPrefs.SetInt("MapaDesdeMenu", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(escenaIntro);
    }
}
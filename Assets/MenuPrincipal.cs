using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private string escenaJuego = "MapaNiveles";

    // BOTÓN JUGAR
    public void Jugar()
    {
        SceneManager.LoadScene(escenaJuego);
    }

    
    
}
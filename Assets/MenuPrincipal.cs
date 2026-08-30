using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private string escenaJuego = "Nivel1";

    // BOTÓN JUGAR
    public void Jugar()
    {
        SceneManager.LoadScene(escenaJuego);
    }

    
    
}
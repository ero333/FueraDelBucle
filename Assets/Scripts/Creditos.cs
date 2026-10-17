using UnityEngine;
using UnityEngine.SceneManagement;
public class Creditos : MonoBehaviour
{
    [Header("Escenas")]
    public string nombreEscenaCreditos = "Creditos";
    public string nombreEscenaMenu = "MenuPrincipal";

    
    public void IrACreditos()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaCreditos);
    }

  
    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}

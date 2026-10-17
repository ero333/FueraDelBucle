using UnityEngine;
using UnityEngine.SceneManagement;

public class CargarEscena : MonoBehaviour
{
    public void Cargar(string nombreEscena)
    {
        if (string.IsNullOrEmpty(nombreEscena)) return;

        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement; 

public class LevelManager : MonoBehaviour
{
    [Header("Número de este nivel (1-10)")]
    public int numeroNivel;

    public void CompletarNivel()
    {
        string clave = "Nivel" + numeroNivel + "Completado";
        PlayerPrefs.SetInt(clave, 1);

        //para desbloquear el siguiente nivel

        int nivelDesbloqueado = PlayerPrefs.GetInt("NivelDesbloqueado", 1);

        if (numeroNivel >= nivelDesbloqueado)
        {
            PlayerPrefs.SetInt("NivelDesbloqueado", numeroNivel + 1);
        }
        PlayerPrefs.Save();

        Debug.Log("Nivel " + numeroNivel + " marcado como completado.");

    }
}

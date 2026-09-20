using UnityEngine;

public class NivelSelector : MonoBehaviour
{
    public GameObject cuadradoCompletado; // asigna en el inspector

    public string claveNivel; 

    void Start()
    {
        // Verifica si el nivel está completado
        if (PlayerPrefs.GetInt(claveNivel, 0) == 1)
        {
            cuadradoCompletado.SetActive(true);
        }
        else
        {
            cuadradoCompletado.SetActive(false);
        }
    }
}


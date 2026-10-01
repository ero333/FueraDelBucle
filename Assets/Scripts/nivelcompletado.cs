using UnityEngine;

public class NivelSelector : MonoBehaviour
{
    public GameObject cuadradoCompletado; // asigna en el inspector
    public string claveNivel;

    void OnEnable()
    {
        ActualizarEstado();

        Debug.Log("Estado de nivel " + claveNivel + ": " + PlayerPrefs.GetInt(claveNivel, 0));
    }

    public void ActualizarEstado()
    {
        // Verifica si el nivel está completado (compara claveNivel exacto)
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


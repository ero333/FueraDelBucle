using UnityEngine;
using UnityEngine.UI;

public class NivelSelector : MonoBehaviour
{
    public GameObject cuadradoCompletado;

    [Header("Sprite de nivel bloqueado")]
    public GameObject spriteBloqueado;

    [Header("Número de este nivel")]
    public int numeroNivel;

    [Header("Clave nivel completado")]
    public string claveNivel;

    void OnEnable()
    {
        ActualizarEstado();
    }

    public void ActualizarEstado()
    {
        Button boton = GetComponent<Button>();
        int nivelDesbloqueado = PlayerPrefs.GetInt("NivelDesbloqueado", 1);

        if (boton != null)
        {
            bool estaDesbloqueado = (numeroNivel <= nivelDesbloqueado);
            boton.interactable = estaDesbloqueado;

            // Mostrar u ocultar sprite de bloqueado
            if (spriteBloqueado != null)
            {
                spriteBloqueado.SetActive(!estaDesbloqueado);
            }

            Debug.Log("Nodo " + gameObject.name + " (Nivel " + numeroNivel + ") -> Desbloqueado: " + estaDesbloqueado + " | Progreso guardado: " + nivelDesbloqueado);
        }

        if (PlayerPrefs.GetInt(claveNivel, 0) == 1)
        {
            if (cuadradoCompletado != null) cuadradoCompletado.SetActive(true);
        }
        else
        {
            if (cuadradoCompletado != null) cuadradoCompletado.SetActive(false);
        }
    }
}
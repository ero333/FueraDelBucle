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

    // Usamos static para que el cheat aplique a todos los botones a la vez
    private static bool cheatDesbloqueoActivo = false;

    void OnEnable()
    {
        ActualizarEstado();
    }

    void Update()
    {
        // Al presionar L, activamos el cheat y actualizamos el estado
        if (Input.GetKeyDown(KeyCode.L))
        {
            cheatDesbloqueoActivo = true;
            ActualizarEstado();
        }
    }

    public void ActualizarEstado()
    {
        Button boton = GetComponent<Button>();
        int nivelDesbloqueado = PlayerPrefs.GetInt("NivelDesbloqueado", 1);

        if (boton != null)
        {
            // El nivel está desbloqueado si su número es menor/igual al guardado, O si el cheat está activo
            bool estaDesbloqueado = (numeroNivel <= nivelDesbloqueado) || cheatDesbloqueoActivo;
            boton.interactable = estaDesbloqueado;

            if (spriteBloqueado != null)
            {
                spriteBloqueado.SetActive(!estaDesbloqueado);
            }
        }

        if (PlayerPrefs.GetInt(claveNivel, 0) == 1 || cheatDesbloqueoActivo)
        {
            if (cuadradoCompletado != null) cuadradoCompletado.SetActive(true);
        }
        else
        {
            if (cuadradoCompletado != null) cuadradoCompletado.SetActive(false);
        }
    }
}
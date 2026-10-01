using UnityEngine;

public class PlataformasPhase : MonoBehaviour
{
    [Header("Jugador")]
    public PlayerPhysics playerPhysics;

    [Header("Plataformas ocultas")]
    public GameObject[] plataformasOcultas;

    private bool estadoAnteriorPhase = false;

    private void Start()
    {
        // Por seguridad, empiezan todas apagadas
        DesactivarPlataformas();

        // Si no asignaste el PlayerPhysics manualmente,
        // intenta encontrarlo en la escena
        if (playerPhysics == null)
        {
            playerPhysics = FindFirstObjectByType<PlayerPhysics>();
        }
    }

    private void Update()
    {
        if (playerPhysics == null)
            return;

        bool phaseActivo = playerPhysics.EstaEnPhase;

        // Solo hacemos cambios cuando cambia el estado del Phase
        if (phaseActivo != estadoAnteriorPhase)
        {
            if (phaseActivo)
            {
                ActivarPlataformas();
            }
            else
            {
                DesactivarPlataformas();
            }

            estadoAnteriorPhase = phaseActivo;
        }
    }

    private void ActivarPlataformas()
    {
        foreach (GameObject plataforma in plataformasOcultas)
        {
            if (plataforma != null)
            {
                plataforma.SetActive(true);
            }
        }
    }

    private void DesactivarPlataformas()
    {
        foreach (GameObject plataforma in plataformasOcultas)
        {
            if (plataforma != null)
            {
                plataforma.SetActive(false);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class DashCooldownUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerPhysics jugador;
    [SerializeField] private Image imagenCooldown;

    void Update()
    {
        if (jugador == null || imagenCooldown == null)
            return;

        // Actualiza la barra según el progreso del cooldown
        imagenCooldown.fillAmount = jugador.ProgresoCooldownDash;
    }
}
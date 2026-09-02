using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DashCooldownUI : MonoBehaviour
{
    [SerializeField] private PlayerPhysics jugador;
    [SerializeField] private Image barraRelleno; // La barra OSCURA (Filled)
    [SerializeField] private TMP_Text textoCooldown; // Opcional, podés dejarlo vacío

    void Update()
    {
        if (jugador == null || barraRelleno == null) return;

        barraRelleno.fillAmount = jugador.ProgresoCooldownDash;

        if (textoCooldown != null)
        {
            if (!jugador.PuedeDashear)
            {
                textoCooldown.text = Mathf.Ceil(jugador.TiempoRestanteCooldown).ToString();
                textoCooldown.gameObject.SetActive(true);
            }
            else
            {
                textoCooldown.gameObject.SetActive(false);
            }
        }
    }
}

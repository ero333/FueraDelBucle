using UnityEngine;
using UnityEngine.UI;

public class PhaseUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerPhysics jugador;
    [SerializeField] private Image imagenRelleno; 

    void Start()
    {
        if (imagenRelleno != null)
        {
            imagenRelleno.type = Image.Type.Filled;
            imagenRelleno.fillMethod = Image.FillMethod.Horizontal;
            imagenRelleno.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
    }

    void Update()
    {
        if (jugador == null || imagenRelleno == null)
            return;

       
        if (jugador.EstaEnPhase)
        {
            imagenRelleno.fillAmount = 1f;
        }
        else
        {
            imagenRelleno.fillAmount = jugador.ProgresoCooldownPhase;
        }
    }
}
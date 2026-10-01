using UnityEngine;

public class InterruptorClick : MonoBehaviour
{
    [Header("Plataforma o camino a activar")]
    public GameObject caminoOculto;

    [Header("Gráficos de la palanca (Opcional)")]
    public Sprite spriteActivado;
    private SpriteRenderer spriteRenderer;

    private bool yaActivado = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        
        if (caminoOculto != null)
        {
            caminoOculto.SetActive(false);
        }
    }


    private void OnMouseDown()
    {
        if (!yaActivado)
        {
            yaActivado = true;


            if (caminoOculto != null)
            {
                caminoOculto.SetActive(true);
            }



            if (spriteActivado != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = spriteActivado;
            }
        }
    }
}

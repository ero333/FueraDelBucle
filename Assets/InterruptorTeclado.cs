using UnityEngine;

public class InterruptorTeclado : MonoBehaviour
{
    public GameObject caminoOculto;
    public Sprite spriteActivado;
    private SpriteRenderer spriteRenderer;

    private bool yaActivado = false;
    private bool jugadorCerca = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (caminoOculto != null)
        {
            caminoOculto.SetActive(false);
        }
    }

    void Update()
    {
        if (jugadorCerca && !yaActivado && Input.GetKeyDown(KeyCode.E))
        {
            yaActivado = true;

            if (caminoOculto != null)
            {
                caminoOculto.SetActive(true);
            }

            // if (spriteActivado != null && spriteRenderer != null)
            // {
            //   spriteRenderer.sprite = spriteActivado;
            // }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}
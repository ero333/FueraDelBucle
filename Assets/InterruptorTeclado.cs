using UnityEngine;

public class InterruptorTeclado : MonoBehaviour
{
    public GameObject[] caminosOcultos;
    public Sprite spriteActivado;

    private Sprite spriteDesactivado;
    private SpriteRenderer spriteRenderer;
    private bool estaActivado = false;
    private bool jugadorCerca = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteDesactivado = spriteRenderer.sprite;
        }

        foreach (GameObject camino in caminosOcultos)
        {
            if (camino != null)
            {
                camino.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            estaActivado = !estaActivado;

            foreach (GameObject camino in caminosOcultos)
            {
                if (camino != null)
                {
                    camino.SetActive(estaActivado);
                }
            }

            if (spriteRenderer != null)
            {
                if (estaActivado && spriteActivado != null)
                {
                    spriteRenderer.sprite = spriteActivado;
                }
                else if (!estaActivado && spriteDesactivado != null)
                {
                    spriteRenderer.sprite = spriteDesactivado;
                }
            }
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
using UnityEngine;

public class InterruptorTeclado : MonoBehaviour
{
    public GameObject[] caminosOcultos;

    public Sprite spriteActivado;

    private Sprite spriteDesactivado;
    private SpriteRenderer spriteRenderer;
    private bool estaActivado = false;
    private bool jugadorCerca = false;

    // Caminos que se desactivan cuando activas el switch
    public GameObject[] caminosDesactivables;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteDesactivado = spriteRenderer.sprite;

            // Arranca desactivado pero mostrando el sprite activado
            if (spriteActivado != null)
            {
                spriteRenderer.sprite = spriteActivado;
            }
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
        if (PauseManager.GameIsPaused || LevelUIManager.PlacaAbierta || Time.timeScale == 0f) return;

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

            foreach (GameObject caminos in caminosDesactivables)
            {
                caminos.SetActive(!estaActivado);
            }

            if (spriteRenderer != null)
            {
                // Función activada = sprite original
                if (estaActivado && spriteDesactivado != null)
                {
                    spriteRenderer.sprite = spriteDesactivado;
                }
                // Función desactivada = sprite activado
                else if (!estaActivado && spriteActivado != null)
                {
                    spriteRenderer.sprite = spriteActivado;
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
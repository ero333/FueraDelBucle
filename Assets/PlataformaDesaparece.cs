using System.Collections;
using UnityEngine;

public class PlataformaDesaparece : MonoBehaviour
{
    [Header("Configuración de la Plataforma")]
    [Tooltip("El tiempo que tarda en destruirse desde que el jugador la pisa.")]
    public float tiempoDeDestruccion = 1.5f;

    private Animator animator;
    private bool yaFuePisada = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player") && !yaFuePisada)
        {
            if (collision.contacts[0].normal.y < -0.5f)
            {
                yaFuePisada = true;
                StartCoroutine(SecuenciaDestruccion());
            }
        }
    }

    private IEnumerator SecuenciaDestruccion()
    {
        if (animator != null)
        {
            animator.SetTrigger("EmpezarRomper");
        }

        yield return new WaitForSeconds(tiempoDeDestruccion);

        Destroy(gameObject);
    }
}
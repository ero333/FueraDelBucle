using UnityEngine;

public class CamaraSeguir : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform objetivo;
    public Vector2 desplazamiento = new Vector2(0f, 1f);

    [Header("Suavizado")]
    [Range(0f, 1f)]
    public float suavizado = 0.15f;

    [Header("Limites del nivel")]
    public bool usarLimites = true;
    public float limiteIzquierdo = 0f;
    public float limiteDerecho = 86.5f;
    public float limiteAbajo = -4f;
    public float limiteArriba = 2f;

    private Vector3 velocidadActual;

    private void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 destino = new Vector3(
            objetivo.position.x + desplazamiento.x,
            objetivo.position.y + desplazamiento.y,
            transform.position.z
        );

        if (usarLimites)
        {
            destino.x = Mathf.Clamp(destino.x, limiteIzquierdo, limiteDerecho);
            destino.y = Mathf.Clamp(destino.y, limiteAbajo, limiteArriba);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            destino,
            ref velocidadActual,
            suavizado
        );
    }
}

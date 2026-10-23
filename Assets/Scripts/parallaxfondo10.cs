using UnityEngine;

public class parallaxfondo10 : MonoBehaviour
{
    public Transform camara;
    [Range(0f, 1f)]
    public float intensidadParallax = 0.5f;

    private Vector3 posicionAnteriorCamara;

    void Start()
    {
        if (camara == null)
            camara = Camera.main.transform;

        posicionAnteriorCamara = camara.position;
    }

    void LateUpdate()
    {
        Vector3 movimiento = camara.position - posicionAnteriorCamara;

        transform.position += new Vector3(
            movimiento.x * intensidadParallax,
            movimiento.y * intensidadParallax,
            0f
        );

        posicionAnteriorCamara = camara.position;
    }
}
using System.Collections;
using UnityEngine;

public class AnimacionFinalPortal : MonoBehaviour
{
    [Header("Referencias")]
    public DialogoLog sistemaDialogo;
    public Transform centroPortal;
    public GameObject pantallaVictoria; // Arrastra tu canvas/panel de victoria aquí

    [Header("Configuración de Animación")]
    public float duracionAnimacion = 2.0f;

    private Renderer miRenderer;
    private Material miMaterial;
    private bool animacionIniciada = false;

    private void OnEnable()
    {
        if (sistemaDialogo != null)
        {
            sistemaDialogo.AlTerminarDialogo += OnDialogoTerminado;
        }
    }

    private void OnDisable()
    {
        if (sistemaDialogo != null)
        {
            sistemaDialogo.AlTerminarDialogo -= OnDialogoTerminado;
        }
    }

    private void Start()
    {
        miRenderer = GetComponentInChildren<Renderer>();
        if (miRenderer != null)
        {
            miMaterial = miRenderer.material;
        }

        // Asegurarnos de que la pantalla de victoria empiece oculta
        if (pantallaVictoria != null)
        {
            pantallaVictoria.SetActive(false);
        }
    }

    private void OnDialogoTerminado()
    {
        if (!animacionIniciada)
        {
            StartCoroutine(EfectoPortalRoutine());
        }
    }

    private IEnumerator EfectoPortalRoutine()
    {
        animacionIniciada = true;

        // Desactivar controles
        var controller = GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Vector3 posicionInicial = transform.position;
        Vector3 escalaInicial = transform.localScale;
        float tiempo = 0f;

        while (tiempo < duracionAnimacion)
        {
            // USAMOS unscaledDeltaTime por si el juego está pausado
            tiempo += Time.unscaledDeltaTime;
            float progreso = Mathf.Clamp01(tiempo / duracionAnimacion);

            // 1. Mover al portal
            if (centroPortal != null)
            {
                transform.position = Vector3.Lerp(posicionInicial, centroPortal.position, progreso);
            }

            // 2. Encoger
            transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, progreso);

            // 3. Disolver
            if (miMaterial != null)
            {
                miMaterial.SetFloat("_DissolveAmount", progreso);
            }

            yield return null;
        }

        // Ocultar personaje
        gameObject.SetActive(false);

        // AHORA SÍ: Mostrar la pantalla de victoria
        if (pantallaVictoria != null)
        {
            pantallaVictoria.SetActive(true);
        }
    }
}
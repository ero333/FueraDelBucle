using UnityEngine;

// Va en el proyectil. Suena al aparecer, pero sólo si el disparo salió dentro de la pantalla, así
// no se escuchan los repeaters que están en otra parte del nivel.
[RequireComponent(typeof(AudioSource))]
public class SonidoProyectil : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField, Range(0f, 1f)] private float volumenDisparo = 0.6f;
    [Tooltip("Cuánto margen fuera de la pantalla se sigue escuchando. 0 = justo hasta el borde.")]
    [SerializeField, Range(0f, 0.5f)] private float margenPantalla = 0.08f;

    private void Awake()
    {
        AudioSource fuente = GetComponent<AudioSource>();

        if (fuente == null) return;

        fuente.playOnAwake = false;

        if (!EnPantalla()) return;

        fuente.volume = Volumen.DeEfecto(volumenDisparo);

        if (fuente.volume <= 0f) return;

        fuente.Play();
    }

    private bool EnPantalla()
    {
        Camera camara = Camera.main;

        if (camara == null) return true;

        Vector3 v = camara.WorldToViewportPoint(transform.position);

        if (v.z < 0f) return false;

        return v.x > -margenPantalla && v.x < 1f + margenPantalla
            && v.y > -margenPantalla && v.y < 1f + margenPantalla;
    }
}

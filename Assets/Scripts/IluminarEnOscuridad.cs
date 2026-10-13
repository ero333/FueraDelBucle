using UnityEngine;
using UnityEngine.Rendering.Universal;

// Lo agrega PaneoPresentacion (por código) al objeto que presenta el paneo, por ejemplo un Repeater.
// En los niveles a oscuras ese objeto se ve casi negro: sus sprites usan un material con luz y la luz global baja.
// Mientras dura la presentación se le cambia el material por uno sin luz y se hace un fade desde cómo se ve a
// oscuras hasta su color normal. Al terminar se hace el camino inverso y se devuelve el material original.
// Si el nivel no tiene el control de oscuridad (LuXVisibilidad) no hace nada.
public class IluminarEnOscuridad : MonoBehaviour
{
    private const string RutaMaterialSinLuz = "Presentacion/SpriteSinLuz";
    private const string TagOscuridad = "Oscuridad";

    private static Material materialSinLuz;

    private SpriteRenderer[] renderers;
    private Material[] materialesOriginales;
    private Color[] coloresOriginales;
    private Light2D luzGlobal;

    private bool cambiado;
    private float objetivo;
    private float avance;
    private float duracion = 0.8f;

    // Devuelve el componente de 'objeto' (lo crea apagado si no lo tiene).
    public static IluminarEnOscuridad Obtener(GameObject objeto)
    {
        IluminarEnOscuridad existente = objeto.GetComponent<IluminarEnOscuridad>();

        if (existente != null) return existente;

        IluminarEnOscuridad nuevo = objeto.AddComponent<IluminarEnOscuridad>();
        nuevo.enabled = false;

        return nuevo;
    }

    // Empieza el fade hacia el color normal.
    public void Aparecer(float segundos)
    {
        if (!HayOscuridad()) return;

        if (!cambiado && !Cambiar()) return;

        objetivo = 1f;
        duracion = Mathf.Max(0.01f, segundos);
        enabled = true;
    }

    // Empieza el fade de vuelta a como se veía a oscuras; al llegar devuelve el material original.
    public void Ocultar(float segundos)
    {
        if (!cambiado) return;

        objetivo = 0f;
        duracion = Mathf.Max(0.01f, segundos);
        enabled = true;
    }

    private void Update()
    {
        if (!cambiado)
        {
            enabled = false;
            return;
        }

        // Si el paneo se corta antes de tiempo, no se queda iluminado para siempre.
        if (objetivo > 0f && !CamaraSeguir.PaneoActivo)
        {
            objetivo = 0f;
        }

        avance = Mathf.MoveTowards(avance, objetivo, Time.deltaTime / duracion);
        Aplicar();

        if (avance <= 0f && objetivo <= 0f)
        {
            Restaurar();
            enabled = false;
        }
    }

    private void OnDisable()
    {
        if (cambiado) Restaurar();
    }

    private bool HayOscuridad()
    {
        if (luzGlobal == null)
        {
            GameObject objetoLuz = GameObject.FindGameObjectWithTag(TagOscuridad);

            if (objetoLuz != null) luzGlobal = objetoLuz.GetComponent<Light2D>();
        }

        return luzGlobal != null && FindFirstObjectByType<LuXVisibilidad>() != null;
    }

    private bool Cambiar()
    {
        if (materialSinLuz == null) materialSinLuz = Resources.Load<Material>(RutaMaterialSinLuz);

        if (materialSinLuz == null)
        {
            Debug.LogWarning("IluminarEnOscuridad: no se encontró el material '" + RutaMaterialSinLuz + "' en Resources.", this);
            return false;
        }

        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        materialesOriginales = new Material[renderers.Length];
        coloresOriginales = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            materialesOriginales[i] = renderers[i].sharedMaterial;
            coloresOriginales[i] = renderers[i].color;
            renderers[i].sharedMaterial = materialSinLuz;
        }

        avance = 0f;
        cambiado = true;
        Aplicar();

        return true;
    }

    // Con el material sin luz el color lo da el tinte: va desde el brillo que tiene la luz del nivel hasta 1.
    private void Aplicar()
    {
        float luz = luzGlobal != null ? Mathf.Clamp01(luzGlobal.intensity) : 1f;
        float k = Mathf.Clamp01(avance);
        float brillo = Mathf.Lerp(luz, 1f, k * k * (3f - 2f * k));

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;

            Color c = coloresOriginales[i];
            renderers[i].color = new Color(c.r * brillo, c.g * brillo, c.b * brillo, c.a);
        }
    }

    private void Restaurar()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;

            renderers[i].sharedMaterial = materialesOriginales[i];
            renderers[i].color = coloresOriginales[i];
        }

        cambiado = false;
        avance = 0f;
        objetivo = 0f;
    }
}

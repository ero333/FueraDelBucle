using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Muestra en el mapa de niveles cuáles ya se completaron: baja el brillo del
// botón y le pone un glow violeta suave detrás. No hace falta agregarlo a la
// escena: se crea solo al cargar "MapaNiveles". Si lo agregás a mano a un
// objeto de la escena, se usan los valores que pongas en el Inspector.
public class MapaNivelesProgreso : MonoBehaviour
{
    private const string NombreEscenaMapa = "MapaNiveles";

    [Header("Nombres")]
    [Tooltip("Los nodos del mapa se llaman prefijo + número (NodoNivel1, NodoNivel2...).")]
    public string prefijoNodo = "NodoNivel";

    [Tooltip("Las escenas de nivel se llaman prefijo + número (Nivel1, Nivel2...).")]
    public string prefijoEscena = "Nivel";

    public int cantidadNiveles = 10;

    [Header("Nivel completado")]
    [Tooltip("Brillo del botón (1 = normal, 0 = negro).")]
    [Range(0f, 1f)]
    public float brillo = 0.55f;

    [Tooltip("Color del glow. El alfa controla qué tan fuerte se ve.")]
    public Color colorGlow = new Color(0.65f, 0.35f, 1f, 0.6f);

    [Tooltip("Tamaño del glow respecto al botón (1 = igual de grande).")]
    public float escalaGlow = 1.5f;

    private Sprite spriteGlow;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Registrar()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private static void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (escena.name != NombreEscenaMapa) return;
        if (FindFirstObjectByType<MapaNivelesProgreso>() != null) return;

        new GameObject("ProgresoMapaNiveles").AddComponent<MapaNivelesProgreso>();
    }

    private void Start()
    {
        for (int i = 1; i <= cantidadNiveles; i++)
        {
            if (!ProgresoNiveles.EstaCompletado(prefijoEscena + i)) continue;

            GameObject nodo = GameObject.Find(prefijoNodo + i);

            if (nodo != null) MarcarComoCompletado(nodo);
        }
    }

    private void MarcarComoCompletado(GameObject nodo)
    {
        Image imagen = nodo.GetComponent<Image>();
        RectTransform rectNodo = nodo.GetComponent<RectTransform>();

        if (imagen == null || rectNodo == null) return;

        imagen.color = new Color(brillo, brillo, brillo, imagen.color.a);

        // El glow es un hermano puesto justo antes del botón, así se dibuja
        // detrás de él (un hijo se dibujaría encima).
        GameObject glow = new GameObject("GlowCompletado", typeof(RectTransform), typeof(Image));
        RectTransform rectGlow = glow.GetComponent<RectTransform>();

        rectGlow.SetParent(rectNodo.parent, false);
        rectGlow.SetSiblingIndex(rectNodo.GetSiblingIndex());

        rectGlow.anchorMin = rectNodo.anchorMin;
        rectGlow.anchorMax = rectNodo.anchorMax;
        rectGlow.pivot = rectNodo.pivot;
        rectGlow.anchoredPosition = rectNodo.anchoredPosition;
        rectGlow.sizeDelta = rectNodo.sizeDelta;
        rectGlow.localScale = rectNodo.localScale * escalaGlow;

        Image imagenGlow = glow.GetComponent<Image>();
        imagenGlow.sprite = ObtenerSpriteGlow();
        imagenGlow.color = colorGlow;
        imagenGlow.raycastTarget = false;
    }

    // Círculo suave: opaco en el centro y transparente en el borde.
    private Sprite ObtenerSpriteGlow()
    {
        if (spriteGlow != null) return spriteGlow;

        const int tamano = 64;

        Texture2D textura = new Texture2D(tamano, tamano, TextureFormat.RGBA32, false);
        textura.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < tamano; y++)
        {
            for (int x = 0; x < tamano; x++)
            {
                float dx = (x + 0.5f) / tamano * 2f - 1f;
                float dy = (y + 0.5f) / tamano * 2f - 1f;
                float distancia = Mathf.Sqrt(dx * dx + dy * dy);
                float alfa = Mathf.Pow(Mathf.Clamp01(1f - distancia), 2f);

                textura.SetPixel(x, y, new Color(1f, 1f, 1f, alfa));
            }
        }

        textura.Apply();

        spriteGlow = Sprite.Create(textura, new Rect(0, 0, tamano, tamano), new Vector2(0.5f, 0.5f), 100f);
        return spriteGlow;
    }
}

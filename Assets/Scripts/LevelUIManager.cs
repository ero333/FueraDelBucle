using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{
    [Header("UI Objetivo")]
    [SerializeField] private GameObject objectivePanel;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private Button closeButton;

    [Header("Configuración del Nivel")]
    [TextArea(3, 5)]
    [SerializeField] private string levelObjective = "¡Llega a la puerta para ganar!";
    [SerializeField] private bool pauseGameOnStart = true;

    [Header("Mientras la placa está abierta")]
    [Tooltip("Cuánto se oscurece todo lo que está detrás de la placa (0 = nada, 1 = negro).")]
    [Range(0f, 1f)]
    [SerializeField] private float oscurecerFondo = 0.6f;

    private GameObject velo;
    private Canvas canvasVelo;
    private Canvas canvasPlaca;
    private int ordenOriginalPlaca;

    private static LevelUIManager instancia;

    // PlayerPhysics lo lee para bloquear el movimiento mientras la placa está en pantalla.
    public static bool PlacaAbierta =>
        instancia != null && instancia.objectivePanel != null && instancia.objectivePanel.activeInHierarchy;

    private void Awake()
    {
        instancia = this;
    }

    private void Start()
    {
        // Asignar el texto e inicializar la UI
        if (objectiveText != null)
            objectiveText.text = levelObjective;

        if (objectivePanel != null)
            objectivePanel.SetActive(true);

        // Escuchar el evento del botón de cerrar
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseObjectivePanel);

        // Pausar el juego si está configurado
        if (pauseGameOnStart)
            Time.timeScale = 0f;
    }

    private void Update()
    {
        // Se lee el estado real de la placa: el velo se quita apenas se cierra,
        // sin importar quién la haya cerrado.
        if (PlacaAbierta && oscurecerFondo > 0f)
        {
            if (velo == null && !CrearVelo())
                return;

            if (!velo.activeSelf)
                MostrarVelo();
        }
        else if (velo != null && velo.activeSelf)
        {
            OcultarVelo();
        }
    }

    private void MostrarVelo()
    {
        // Órdenes explícitos (los Canvas con el mismo orden se empatan y no se sabe cuál queda arriba):
        // resto de la UI (vidas, pausa, dash) < velo < placa.
        int maximo = canvasPlaca.sortingOrder;

        foreach (Canvas otro in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            if (!otro.isRootCanvas || otro == canvasPlaca || otro == canvasVelo)
                continue;

            maximo = Mathf.Max(maximo, otro.sortingOrder);
        }

        ordenOriginalPlaca = canvasPlaca.sortingOrder;

        canvasVelo.sortingLayerID = canvasPlaca.sortingLayerID;
        canvasVelo.sortingOrder = maximo + 1;
        canvasPlaca.sortingOrder = maximo + 2;

        velo.SetActive(true);
    }

    private void OcultarVelo()
    {
        velo.SetActive(false);

        // La placa vuelve a su orden original.
        if (canvasPlaca != null)
            canvasPlaca.sortingOrder = ordenOriginalPlaca;
    }

    private bool CrearVelo()
    {
        Canvas canvas = objectivePanel.GetComponentInParent<Canvas>();
        if (canvas == null)
            return false;

        canvasPlaca = canvas.rootCanvas;

        // Canvas propio a pantalla completa: la placa, las vidas y el menú de pausa son Canvas distintos.
        velo = new GameObject("VeloPlaca", typeof(Canvas), typeof(Image));

        canvasVelo = velo.GetComponent<Canvas>();
        canvasVelo.renderMode = RenderMode.ScreenSpaceOverlay;

        Image imagen = velo.GetComponent<Image>();
        imagen.color = new Color(0f, 0f, 0f, oscurecerFondo);
        imagen.raycastTarget = false; // no bloquea clics

        // Nace apagado: MostrarVelo lo ordena y lo enciende.
        velo.SetActive(false);

        return true;
    }

    public void MostrarObjetivo()
    {
        if (objectivePanel == null)
            return;

        objectivePanel.transform.SetAsLastSibling();
        objectivePanel.SetActive(true);
    }

    public void CloseObjectivePanel()
    {
        if (objectivePanel != null)
            objectivePanel.SetActive(false);

        if (!PauseManager.GameIsPaused)
            Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        if (instancia == this)
            instancia = null;

        // Buena práctica: limpiar el oyente al cambiar de escena
        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseObjectivePanel);
    }
}

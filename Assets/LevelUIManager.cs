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

    public void CloseObjectivePanel()
    {
        if (objectivePanel != null)
            objectivePanel.SetActive(false);

        // Reanudar el paso del tiempo
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        // Buena práctica: limpiar el oyente al cambiar de escena
        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseObjectivePanel);
    }
}

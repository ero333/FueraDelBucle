using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Analytics;
using System.Collections.Generic;

public class CompletarNivel : MonoBehaviour
{
    [Header("Configuración de Nivel")]
    [Tooltip("Debe coincidir exactamente con la 'Clave Nivel' del menú.")]
    public string claveNivel = "Nivel1";

    [Header("Opciones de Navegación")]
    public string nombreEscenaMenu = "MapaNiveles";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GanarNivel();
        }
    }

    public void GanarNivel()
    {
        CustomEvent levelCompleteEvent = new CustomEvent("LevelComplete")
        {
            { "Level", 1 },
            { "MusicActive", true },
            { "SoundActive", true },
            { "TimeElapsed", 120.5f },
            { "PhaseAvoidances", 0 },
            { "DashCount", 0 },
            { "PushCount", 0 },
            { "KillCount", 0 }
        };

        AnalyticsService.Instance.RecordEvent(levelCompleteEvent);
        AnalyticsService.Instance.Flush();

        if (!string.IsNullOrEmpty(nombreEscenaMenu))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nombreEscenaMenu);
        }
    }
}
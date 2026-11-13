using System;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UnityConsent;

public class IniciarAnalytics : MonoBehaviour
{
    async void Start()
    {
        try
        {
            // 1. Inicializa el SDK de Unity Services
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                await UnityServices.InitializeAsync();
            }

            // 2. Establece el consentimiento (esto inicia la recolección automáticamente)
            EstablecerConsentimiento(true);

            Debug.Log("[Analytics] Inicializado y consentimiento otorgado correctamente.");

            // 3. Libera la cola de eventos que se dispararon al cargar la escena (incluyendo el primer LevelStart)
            Eventos.NotificarServiciosListos();
        }
        catch (Exception e)
        {
            Debug.LogError($"[Analytics] Error al inicializar: {e.Message}");
        }
    }

    public void EstablecerConsentimiento(bool tieneConsentimiento)
    {
        ConsentState estadoConsentimiento = new ConsentState
        {
            AnalyticsIntent = tieneConsentimiento ? ConsentStatus.Granted : ConsentStatus.Denied,
            AdsIntent = ConsentStatus.Denied
        };

        EndUserConsent.SetConsentState(estadoConsentimiento);
    }
}
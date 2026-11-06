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
            await UnityServices.InitializeAsync();

            // 2. Establece el consentimiento (Analytics y Anuncios)
            EstablecerConsentimiento(true);

            // 3. INICIA LA RECOLECCIÓN (Obligatorio para enviarle datos a Unity)
            AnalyticsService.Instance.StartDataCollection();

            Debug.Log("[Analytics] Inicializado y recolectando datos correctamente.");

            // 4. (Opcional) Registrar el primer nivel de forma segura
            int nivelActual = Eventos.NivelActual();
            if (nivelActual != Eventos.SinNivel)
            {
                Eventos.LogLevelStart(nivelActual);
            }
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
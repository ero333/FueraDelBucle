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
            // 1. Inicializa el SDK de Unity Services (se vincula automáticamente a tu Project ID)
            await UnityServices.InitializeAsync();

            // 2. Establece el consentimiento
            EstablecerConsentimiento(true);

            // 3. Establece el consentimiento para iniciar la recolección de métricas
            EndUserConsent.SetConsentState(new ConsentState
            {
                AnalyticsIntent = ConsentStatus.Granted
            });

            Debug.Log("Analytics inicializado correctamente.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al inicializar Unity Analytics: {e.Message}");
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
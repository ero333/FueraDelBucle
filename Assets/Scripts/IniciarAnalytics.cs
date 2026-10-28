using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UnityConsent;

public class IniciarAnalytics : MonoBehaviour
{
  
   
        async void Start()
    {
        await UnityServices.InitializeAsync();
    }

    public void ejemplo2()
    {
        bool consentimiento = true;
        ConsentState estadoConsentimiento = new ConsentState
        {
            //  En el caso de que queramos utilizar los analytics para la recolección de datos, le decimos a la variable
            //AnalyticsIntent = ConsentStatus.Granted. // Osea, aca le decimos que el usuario dio su consentimiento
            //  Caso de que queramos decirle que no damos consentimiento, usamos  AnalyticsIntent = ConsentStatus.Denied
            AnalyticsIntent = consentimiento ? ConsentStatus.Granted : ConsentStatus.Denied, // (aviso por las dudas, cuando usamos ‘?’ es como un if/else)
            AdsIntent = ConsentStatus.Denied
            // Este caso de aca a lo último es para dar consentimiento o no para el uso de anuncios. En este caso le decimos que no
        };

        // Y despues usamos la siguiente funcion para avisar a los servicios de Unity que el usuario dio su consentimiento.
        EndUserConsent.SetConsentState(estadoConsentimiento);

    }
}

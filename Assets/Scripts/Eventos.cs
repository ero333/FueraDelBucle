using Unity.Services.Analytics;
using UnityEngine;


public class Eventos : MonoBehaviour
{
    public static Eventos Singleton;
    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
    }

    public void EventoLevel_Start(int numeroNivelActual)
    {
        CustomEvent nombre/* de la variable(ejemplo level_start) */= new CustomEvent("noombre del evento de Unity")
        {
            { "nombre del parametro",numeroNivelActual }//agregar numero o el parametro que corresponda
        }
        ;

        //AnalyticsService.Instance.RecordEvent(nombre);
       
        //print(nombre del evento parametro = "" + nomeroNivelActual);

    }
    //Enviar el evento a cada codigo que corresponda
}
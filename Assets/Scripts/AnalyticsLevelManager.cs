using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement; 

//[{public class AnalyticsLevelManager : MonoBehaviour
//{
  //  private async void Start()
    //{
       
      //  if (UnityServices.State != ServicesInitializationState.Initialized)
        //{
          //  await UnityServices.InitializeAsync();
        //}

        
    //    RegistrarLevelStartActual();
   // }

    //private void RegistrarLevelStartActual()
    //{
       
      //  Scene escenaActual = SceneManager.GetActiveScene();

        //string nombreNivel = escenaActual.name;         
       // int numeroNivel = escenaActual.buildIndex;      

        
       //CustomEvent levelStartEvent = new CustomEvent("LevelStart")
     //  {
    //        { "levelName", nombreNivel },
    //        { "levelIndex", numeroNivel }
    //   }
    //   ;

   //[     AnalyticsService.Instance.RecordEvent(levelStartEvent);

       
       // Debug.Log($"<color=cyan>[Analytics Event]</color> <b>LevelStart</b> enviado con éxito \n" +
         //         $"- Nombre del Nivel: <b>{nombreNivel}</b>\n" +
           //       $"- Índice (Build Index): <b>{numeroNivel}</b>");
  //  }
//}
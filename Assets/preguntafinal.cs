using UnityEngine;

public class preguntafinal : MonoBehaviour
{
    [Header("Referencias del Diálogo")]
    [Tooltip("Arrastra aquí el objeto de la escena que tiene el script DialogoLog.")]
    public DialogoLog dialogoLog;

    [Tooltip("Arrastra aquí el archivo ScriptableObject 'Ultimapregunta'.")]
    public NPCDialogos Ultimapregunta;

   
    public void MostrarPregunta()
    {
        if (dialogoLog != null && Ultimapregunta != null)
        {
            // Asigna los datos de Ultimapregunta al componente DialogoLog
            dialogoLog.dialogodata = Ultimapregunta;

            // Inicia la animación y despliega el diálogo
            dialogoLog.EmpezarDialog();
        }
        else
        {
            Debug.LogWarning("Falta asignar dialogoLog o Ultimapregunta en el Inspector de preguntafinal.");
        }
    }
}
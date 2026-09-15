using UnityEngine;

public class TriggerDialogoInicio : MonoBehaviour
{
    public DialogoLog dialogoLog;

    private bool dialogoEnCurso;

    void Start()
    {
        // Congela el juego (fisica, movimiento, enemigos, etc.)
        // El tipeo del dialogo sigue funcionando porque usa WaitForSecondsRealtime.
        Time.timeScale = 0f;
        dialogoEnCurso = true;

        dialogoLog.Interactuar();
    }

    void Update()
    {
        if (!dialogoEnCurso) return;

        // DialogoLog desactiva PanelDialogo cuando el dialogo termina (TerminarDialog).
        // Lo usamos como señal externa sin tener que modificar DialogoLog.cs.
        if (!dialogoLog.PanelDialogo.activeSelf)
        {
            Time.timeScale = 1f;
            dialogoEnCurso = false;
        }
    }
}
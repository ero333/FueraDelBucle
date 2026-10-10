using UnityEngine;

public class TriggerDialogoInicio : MonoBehaviour
{
    public DialogoLog dialogoLog;

    void Start()
    {
        if (dialogoLog == null) return;

        dialogoLog.Interactuar();
    }
}

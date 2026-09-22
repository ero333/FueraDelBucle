using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogoLog : MonoBehaviour
{
    public NPCDialogos dialogodata;
    public GameObject PanelDialogo;
    public TMP_Text dialogoTexto, NombreText;
    public Image RetratoAnim;

    [Header("Teclas para pasar el dialogo")]
    [Tooltip("Tecla principal para pasar a la linea siguiente.")]
    public KeyCode teclaAvanzar = KeyCode.Return;

    [Tooltip("Tecla alternativa, la misma que se usa para interactuar.")]
    public KeyCode teclaAvanzarAlternativa = KeyCode.E;

    [Tooltip("Tecla para saltear el dialogo entero y cerrarlo, como el boton de la X.")]
    public KeyCode teclaSaltarTodo = KeyCode.X;

    // Evento que escucha PanelVictoria para saber cuándo se cierra el cartel
    public event Action AlTerminarDialogo;

    private int dialogoIndex;
    private bool EstaTypeando, DialogoActivo;
    private CanvasGroup grupoPanel;
    private bool ocultoPorPausa;

    private void Update()
    {
        ActualizarVisibilidadPorPausa();

        if (!DialogoActivo) return;
        if (PauseManager.GameIsPaused) return;

        if (Input.GetKeyDown(teclaSaltarTodo))
        {
            SkipearDialogoCompleto();
            return;
        }

        if (Input.GetKeyDown(teclaAvanzar)
            || Input.GetKeyDown(KeyCode.KeypadEnter)
            || Input.GetKeyDown(teclaAvanzarAlternativa))
        {
            Interactuar();
        }
    }

    public void Interactuar()
    {
        if (PauseManager.GameIsPaused) return;

        if (DialogoActivo)
        {
            SigLinea();
        }
        else
        {
            EmpezarDialog();
        }
    }

    void EmpezarDialog()
    {
        DialogoActivo = true;
        dialogoIndex = 0;

        NombreText.SetText(dialogodata.NombreNPC);
        RetratoAnim.sprite = dialogodata.LogRetrato;

        PanelDialogo.SetActive(true);

        StartCoroutine(LineadeType());
    }

    /// <summary>
    /// Si está escribiendo una línea, la completa de inmediato.
    /// Si el texto ya está completo y se vuelve a presionar Skip/Interactuar, avanza a la siguiente o termina.
    /// </summary>
    public void SkipearAnimacion()
    {
        if (PauseManager.GameIsPaused) return;
        if (!DialogoActivo) return;

        if (EstaTypeando)
        {
            StopAllCoroutines();
            dialogoTexto.SetText(dialogodata.lineasDialogo[dialogoIndex]);
            EstaTypeando = false;

            ProcesarAutoProgresion();
        }
        else
        {
            SigLinea();
        }
    }

    /// <summary>
    /// Salta todo el diálogo restante inmediatamente y activa el cierre/evento final.
    /// Úsalo en un botón de 'Skip Total' o para omitir la cinemática.
    /// </summary>
    public void SkipearDialogoCompleto()
    {
        if (PauseManager.GameIsPaused) return;
        if (!DialogoActivo) return;

        TerminarDialog();
    }

    void SigLinea()
    {
        if (EstaTypeando)
        {
            StopAllCoroutines();
            dialogoTexto.SetText(dialogodata.lineasDialogo[dialogoIndex]);
            EstaTypeando = false;
            return;
        }
        else if (dialogoIndex + 1 < dialogodata.lineasDialogo.Length)
        {
            // Si hay otra línea de texto, tipea la siguiente
            dialogoIndex++;
            StartCoroutine(LineadeType());
        }
        else
        {
            TerminarDialog();
        }
    }

    IEnumerator LineadeType()
    {
        EstaTypeando = true;
        dialogoTexto.SetText("");

        foreach (char letter in dialogodata.lineasDialogo[dialogoIndex])
        {
            dialogoTexto.text += letter;
            yield return EsperarTiempoRespetandoPausa(dialogodata.velocidadTypeo);
        }

        EstaTypeando = false;

        if (DebeAutoProgresar(dialogoIndex))
        {
            yield return EsperarTiempoRespetandoPausa(dialogodata.autoProgresDelay);
            yield return new WaitUntil(() => !PauseManager.GameIsPaused);
            SigLinea();
        }
    }

    void ProcesarAutoProgresion()
    {
        if (DebeAutoProgresar(dialogoIndex))
        {
            StartCoroutine(PasarSigLineaXTiempo());
        }
    }

    IEnumerator PasarSigLineaXTiempo()
    {
        yield return EsperarTiempoRespetandoPausa(dialogodata.autoProgresDelay);
        yield return new WaitUntil(() => !PauseManager.GameIsPaused);

        if (dialogoIndex + 1 < dialogodata.lineasDialogo.Length)
        {
            dialogoIndex++;
            StartCoroutine(LineadeType());
        }
        else
        {
            TerminarDialog();
        }
    }

    IEnumerator EsperarTiempoRespetandoPausa(float tiempo)
    {
        float timer = 0f;
        while (timer < tiempo)
        {
            if (!PauseManager.GameIsPaused)
            {
                timer += Time.unscaledDeltaTime;
            }
            yield return null;
        }
    }

    void ActualizarVisibilidadPorPausa()
    {
        bool debeOcultarse = DialogoActivo && (PauseManager.GameIsPaused || PauseManager.CerrandoEscena);
        if (debeOcultarse == ocultoPorPausa) return;
        MostrarPanel(!debeOcultarse);
    }

    void MostrarPanel(bool visible)
    {
        ocultoPorPausa = !visible;
        if (PanelDialogo == null) return;

        if (grupoPanel == null)
        {
            grupoPanel = PanelDialogo.GetComponent<CanvasGroup>();
            if (grupoPanel == null) grupoPanel = PanelDialogo.AddComponent<CanvasGroup>();
        }

        grupoPanel.alpha = visible ? 1f : 0f;
        grupoPanel.interactable = visible;
        grupoPanel.blocksRaycasts = visible;
    }

    bool DebeAutoProgresar(int index)
    {
        return dialogodata.autoProgresLineas != null
            && index < dialogodata.autoProgresLineas.Length
            && dialogodata.autoProgresLineas[index];
    }

    public void TerminarDialog()
    {
        if (ocultoPorPausa) MostrarPanel(true);

        StopAllCoroutines();
        DialogoActivo = false;
        dialogoTexto.SetText("");
        PanelDialogo.SetActive(false);

        // Notificar a PanelVictoria que el diálogo concluyó
        AlTerminarDialogo?.Invoke();
    }
}
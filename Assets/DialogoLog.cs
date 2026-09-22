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

    // Evento que escucha PanelVictoria para saber cuándo se cierra el cartel
    public event Action AlTerminarDialogo;

    private int dialogoIndex;
    private bool EstaTypeando, DialogoActivo;

    public void Interactuar()
    {
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
            yield return new WaitForSecondsRealtime(dialogodata.velocidadTypeo);
        }

        EstaTypeando = false;

        if (DebeAutoProgresar(dialogoIndex))
        {
            yield return new WaitForSecondsRealtime(dialogodata.autoProgresDelay);
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
        yield return new WaitForSecondsRealtime(dialogodata.autoProgresDelay);

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

    bool DebeAutoProgresar(int index)
    {
        return dialogodata.autoProgresLineas != null
            && index < dialogodata.autoProgresLineas.Length
            && dialogodata.autoProgresLineas[index];
    }

    public void TerminarDialog()
    {
        StopAllCoroutines();
        DialogoActivo = false;
        dialogoTexto.SetText("");
        PanelDialogo.SetActive(false);

        // Notificar a PanelVictoria que el diálogo concluyó
        AlTerminarDialogo?.Invoke();
    }
}
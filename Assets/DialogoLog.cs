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

    public void SkipearAnimacion()
    {
        if (DialogoActivo && EstaTypeando)
        {
            StopAllCoroutines();
            dialogoTexto.SetText(dialogodata.lineasDialogo[dialogoIndex]);
            EstaTypeando = false;

            ProcesarAutoProgresion();
        }
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
            //Si hay otra linea de texto, typea la sig linea
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

    // Chequea si la linea en el indice dado debe auto-progresar,
    // sin romperse si el array autoProgresLineas es mas corto que lineasDialogo
    // (en ese caso, se asume "false" para los indices faltantes).
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

    }
}
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

        foreach(char letter in dialogodata.lineasDialogo[dialogoIndex])
        {
            dialogoTexto.text += letter;
            yield return new WaitForSeconds(dialogodata.velocidadTypeo);
        }

        EstaTypeando = false;

        if (dialogodata.autoProgresLineas.Length > dialogoIndex && dialogodata.autoProgresLineas[dialogoIndex])
        {
            yield return new WaitForSeconds(dialogodata.autoProgresDelay);
            SigLinea();
        }
    }

    void ProcesarAutoProgresion()
    {
        if (dialogodata.autoProgresLineas.Length > dialogoIndex && dialogodata.autoProgresLineas[dialogoIndex]) ;
        {
            StartCoroutine(PasarSigLineaXTiempo());
        }
    }

    IEnumerator PasarSigLineaXTiempo()
    {
        yield return new WaitForSeconds(dialogodata.autoProgresDelay);

        if(dialogoIndex + 1 < dialogodata.lineasDialogo.Length)
        {
            dialogoIndex++;
            StartCoroutine(LineadeType());
        }

        else
        {
            TerminarDialog();
        }
    }

    public void TerminarDialog()
    {
        StopAllCoroutines();
        DialogoActivo = false;
        dialogoTexto.SetText("");
        PanelDialogo.SetActive(false);
        
    }
}

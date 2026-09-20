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

    private int dialogoIndex;
    private bool EstaTypeando, DialogoActivo;

    private void Update()
    {
        if (!DialogoActivo) return;

        if (Input.GetKeyDown(teclaSaltarTodo))
        {
            SkipTodoElDialogo();
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
        if (PauseManager.GameIsPaused) return; // Bloquea la interacción si el juego está pausado

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
        if (PauseManager.GameIsPaused) return;

        if (DialogoActivo && EstaTypeando)
        {
            StopAllCoroutines();
            dialogoTexto.SetText(dialogodata.lineasDialogo[dialogoIndex]);
            EstaTypeando = false;

            ProcesarAutoProgresion();
        }
    }


    public void SkipTodoElDialogo()
    {
        if (PauseManager.GameIsPaused) return;

        if (DialogoActivo)
        {
            TerminarDialog();
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

            // Reemplazo de WaitForSecondsRealtime: Espera el tiempo de typeo considerando la pausa
            yield return EsperarTiempoRespetandoPausa(dialogodata.velocidadTypeo);
        }

        EstaTypeando = false;

        if (DebeAutoProgresar(dialogoIndex))
        {
            yield return EsperarTiempoRespetandoPausa(dialogodata.autoProgresDelay);

            // Verifica que no haya quedado pausado antes de cambiar de línea
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

    // Método auxiliar para contar el tiempo manualmente ignorando la velocidad del juego pero respetando la pausa
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
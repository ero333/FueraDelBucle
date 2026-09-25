using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogoLog : MonoBehaviour
{
    public NPCDialogos dialogodata;
    public GameObject PanelDialogo;

    [Tooltip("Asigna el panel de objetivos de esta escena si existe.")]
    public PopupController popupObjetivo;

    public TMP_Text dialogoTexto, NombreText;
    public Image RetratoAnim;

    [Header("Teclas para pasar el dialogo")]
    public KeyCode teclaAvanzar = KeyCode.Return;
    public KeyCode teclaAvanzarAlternativa = KeyCode.E;
    public KeyCode teclaSaltarTodo = KeyCode.X;

    public event Action AlTerminarDialogo;

    private int dialogoIndex;
    private bool EstaTypeando, DialogoActivo;
    private CanvasGroup grupoPanel;
    private bool ocultoPorPausa;
    private bool esperandoParaProcesarInput = false;

    private void Awake()
    {
        // Buscar el PopupController automáticamente si no está asignado
        if (popupObjetivo == null)
        {
            popupObjetivo = FindFirstObjectByType<PopupController>();
        }
    }

    private void OnEnable()
    {
        if (popupObjetivo != null)
        {
            popupObjetivo.OnPopupClosed += OnObjetivoCerrado;
        }
    }

    private void OnDisable()
    {
        if (popupObjetivo != null)
        {
            popupObjetivo.OnPopupClosed -= OnObjetivoCerrado;
        }
    }

    private void Start()
    {
        // En niveles SIN panel de objetivos, si no dependes de un Trigger externo,
        // puedes iniciar el diálogo directamente si el panel no existe.
        if (popupObjetivo == null && !DialogoActivo)
        {
            // Opcional: Descomenta la siguiente línea si tus niveles sin objetivos deben arrancar el diálogo solo al iniciar.
            // EmpezarDialog();
        }
    }

    private void OnObjetivoCerrado()
    {
        if (popupObjetivo != null && popupObjetivo.esPopupInicial)
        {
            popupObjetivo.esPopupInicial = false;
            // Iniciamos el diálogo pero bloqueamos el Input durante el frame del cierre
            StartCoroutine(IniciarDialogoConCooldown());
        }
    }

    private IEnumerator IniciarDialogoConCooldown()
    {
        esperandoParaProcesarInput = true;
        EmpezarDialog();
        yield return null;
        yield return new WaitForEndOfFrame();
        esperandoParaProcesarInput = false;
    }

    private void Update()
    {
        ActualizarVisibilidadPorPausa();

        // 1. Ignorar entrada si el juego está pausado, el diálogo está inactivo o en cooldown de cierre
        if (!DialogoActivo || PauseManager.GameIsPaused || esperandoParaProcesarInput) return;

        // 2. Bloquear controles si el panel de objetivo sigue visible físicamente
        if (popupObjetivo != null && popupObjetivo.EstaVisible()) return;

        // 3. Saltear TODO el diálogo inmediatamente con X
        if (Input.GetKeyDown(teclaSaltarTodo))
        {
            SkipearDialogoCompleto();
            return;
        }

        // 4. Avanzar texto o pasar a la siguiente línea con Enter / KeypadEnter / E
        if (Input.GetKeyDown(teclaAvanzar)
            || Input.GetKeyDown(KeyCode.KeypadEnter)
            || Input.GetKeyDown(teclaAvanzarAlternativa))
        {
            Interactuar();
        }
    }

    public void Interactuar()
    {
        if (PauseManager.GameIsPaused || esperandoParaProcesarInput) return;

        if (DialogoActivo)
        {
            if (EstaTypeando)
            {
                // Si aún está escribiendo la línea, frena el tipeo y muestra el texto completo
                StopAllCoroutines();
                dialogoTexto.SetText(dialogodata.lineasDialogo[dialogoIndex]);
                EstaTypeando = false;

                // Procesa la auto-progresión si está habilitada para esta línea
                ProcesarAutoProgresion();
            }
            else
            {
                // Si el texto de la línea actual ya terminó, pasa a la siguiente
                SigLinea();
            }
        }
        else
        {
            EmpezarDialog();
        }
    }

    public void EmpezarDialog()
    {
        if (dialogodata == null) return;

        StopAllCoroutines(); // Cancela cualquier corrutina de tipeo o timer residual

        DialogoActivo = true;
        dialogoIndex = 0;

        if (NombreText != null) NombreText.SetText(dialogodata.NombreNPC);
        if (RetratoAnim != null) RetratoAnim.sprite = dialogodata.LogRetrato;

        PanelDialogo.SetActive(true);

        StartCoroutine(LineadeType());
    }

    public void SkipearDialogoCompleto()
    {
        if (PauseManager.GameIsPaused || !DialogoActivo) return;
        TerminarDialog();
    }

    void SigLinea()
    {
        StopAllCoroutines(); // Cancela timers de autoprogresión pendientes

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

        ProcesarAutoProgresion();
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

        if (!EstaTypeando)
        {
            SigLinea();
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
        return dialogodata != null
            && dialogodata.autoProgresLineas != null
            && index < dialogodata.autoProgresLineas.Length
            && dialogodata.autoProgresLineas[index];
    }

    public void TerminarDialog()
    {
        if (ocultoPorPausa) MostrarPanel(true);

        StopAllCoroutines();
        DialogoActivo = false;
        if (dialogoTexto != null) dialogoTexto.SetText("");
        PanelDialogo.SetActive(false);

        AlTerminarDialogo?.Invoke();
    }
}
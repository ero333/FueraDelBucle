using System;
using System.Collections;
using System.Collections.Generic;
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
    public KeyCode teclaSaltarTodoAlternativa = KeyCode.Escape;

    [Tooltip("Dejalo desmarcado para que el jugador pase cada parlamento a mano.")]
    public bool autoProgresar = false;

    public event Action AlTerminarDialogo;

    private int dialogoIndex;
    private bool EstaTypeando, DialogoActivo;
    private CanvasGroup grupoPanel;
    private bool ocultoPorPausa;
    private float tiempoFinCooldown;
    private bool notaEstabaAbierta;
    private Vector2 posicionRetrato;
    private bool guardePosicionRetrato;

    // Cajas de diálogo de la escena. PlayerPhysics pregunta por HayDialogoActivo
    // para bloquear el movimiento del jugador mientras haya un diálogo en pantalla.
    private static readonly List<DialogoLog> instancias = new List<DialogoLog>();

    public static bool HayDialogoActivo
    {
        get
        {
            for (int i = 0; i < instancias.Count; i++)
            {
                DialogoLog dialogo = instancias[i];

                if (dialogo != null && dialogo.DialogoActivo
                    && dialogo.PanelDialogo != null && dialogo.PanelDialogo.activeInHierarchy)
                    return true;
            }

            return false;
        }
    }

    private void Awake()
    {
        // Buscar el PopupController automáticamente si no está asignado
        if (popupObjetivo == null) return;

        if (popupObjetivo.Ventana == PanelDialogo)
        {
            popupObjetivo = null;
        }
    }

    private void OnEnable()
    {
        ConectarBotonCerrar();

        if (!instancias.Contains(this))
            instancias.Add(this);

        if (popupObjetivo != null)
        {
            popupObjetivo.OnPopupClosed += OnObjetivoCerrado;
        }
    }

    private void ConectarBotonCerrar()
    {
        if (PanelDialogo == null) return;

        Button[] botones = PanelDialogo.GetComponentsInChildren<Button>(true);

        for (int i = 0; i < botones.Length; i++)
        {
            string etiqueta = EtiquetaDe(botones[i]);

            if (etiqueta == "X")
            {
                botones[i].onClick.RemoveListener(SkipearDialogoCompleto);
                botones[i].onClick.AddListener(SkipearDialogoCompleto);
            }
            else if (etiqueta == "E")
            {
                botones[i].onClick.RemoveListener(AvanzarDesdeBoton);
                botones[i].onClick.AddListener(AvanzarDesdeBoton);
            }
        }
    }

    private string EtiquetaDe(Button boton)
    {
        if (boton == null) return string.Empty;

        Text texto = boton.GetComponentInChildren<Text>(true);
        if (texto != null && !string.IsNullOrEmpty(texto.text)) return texto.text.Trim().ToUpperInvariant();

        TMP_Text textoTMP = boton.GetComponentInChildren<TMP_Text>(true);
        if (textoTMP != null && !string.IsNullOrEmpty(textoTMP.text)) return textoTMP.text.Trim().ToUpperInvariant();

        return string.Empty;
    }

    private void OnDisable()
    {
        instancias.Remove(this);

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
            IniciarDialogoConCooldown();
        }
    }

    private void IniciarDialogoConCooldown()
    {
        tiempoFinCooldown = Time.unscaledTime + 0.15f;
        EmpezarDialog();
    }

    private void VigilarNotaDeObjetivo()
    {
        if (popupObjetivo == null) return;

        bool abierta = popupObjetivo.EstaVisible();

        if (notaEstabaAbierta && !abierta) OnObjetivoCerrado();

        notaEstabaAbierta = abierta;
    }

    private void Update()
    {
        VigilarNotaDeObjetivo();

        ActualizarVisibilidadPorPausa();

        // 1. Ignorar entrada si el juego está pausado, el diálogo está inactivo o en cooldown de cierre
        if (!DialogoActivo || PauseManager.GameIsPaused || (Time.unscaledTime < tiempoFinCooldown)) return;

        // 2. Bloquear controles si el panel de objetivo sigue visible físicamente
        if (popupObjetivo != null && popupObjetivo.EstaVisible()) return;

        // 3. Saltear TODO el diálogo inmediatamente con X
        if (Input.GetKeyDown(teclaSaltarTodo) || Input.GetKeyDown(teclaSaltarTodoAlternativa))
        {
            SkipearDialogoCompleto();
            return;
        }

        // 4. Avanzar texto o pasar a la siguiente línea con Enter / KeypadEnter / E
        if (Input.GetKeyDown(teclaAvanzar) || Input.GetKeyDown(teclaAvanzarAlternativa)
            || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Interactuar();
        }
    }

    public void AvanzarDesdeBoton()
    {
        if (!DialogoActivo) return;

        Interactuar();
    }

    public void Interactuar()
    {
        if (PauseManager.GameIsPaused || (Time.unscaledTime < tiempoFinCooldown)) return;

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

        ActualizarRetrato();

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

    private void ActualizarRetrato()
    {
        if (RetratoAnim == null || dialogodata == null) return;

        Sprite elegido = dialogodata.LogRetrato;

        if (dialogodata.retratosPorLinea != null
            && dialogoIndex < dialogodata.retratosPorLinea.Length
            && dialogodata.retratosPorLinea[dialogoIndex] != null)
        {
            elegido = dialogodata.retratosPorLinea[dialogoIndex];
        }

        RetratoAnim.sprite = elegido;

        float escala = 1f;

        if (dialogodata.escalaRetratoPorLinea != null
            && dialogoIndex < dialogodata.escalaRetratoPorLinea.Length
            && dialogodata.escalaRetratoPorLinea[dialogoIndex] > 0f)
        {
            escala = dialogodata.escalaRetratoPorLinea[dialogoIndex];
        }

        RectTransform rectRetrato = RetratoAnim.rectTransform;

        if (!guardePosicionRetrato)
        {
            posicionRetrato = rectRetrato.anchoredPosition;
            guardePosicionRetrato = true;
        }

        rectRetrato.localScale = Vector3.one * escala;

        float crecimiento = (escala - 1f) * rectRetrato.rect.height * 0.5f;

        float corrimiento = 0f;

        if (dialogodata.desplazamientoRetratoPorLinea != null
            && dialogoIndex < dialogodata.desplazamientoRetratoPorLinea.Length)
        {
            corrimiento = dialogodata.desplazamientoRetratoPorLinea[dialogoIndex];
        }

        rectRetrato.anchoredPosition = posicionRetrato + new Vector2(corrimiento, crecimiento);
    }

    IEnumerator LineadeType()
    {
        ActualizarRetrato();

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
        bool notaAbierta = LevelUIManager.PlacaAbierta;
        bool debeOcultarse = DialogoActivo && (PauseManager.GameIsPaused || PauseManager.CerrandoEscena || notaAbierta);
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
        if (!autoProgresar) return false;

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
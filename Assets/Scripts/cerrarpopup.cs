using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    [SerializeField] private GameObject ventanaPopup;

    public event Action OnPopupClosed;

    public bool esPopupInicial { get; set; } = true;

    public GameObject Ventana { get { return ventanaPopup; } }

    [Header("Teclas para cerrar la nota")]
    public KeyCode teclaCerrar = KeyCode.Return;
    public KeyCode teclaCerrarAlternativa = KeyCode.X;
    public KeyCode teclaCerrarAlternativa2 = KeyCode.E;
    public KeyCode teclaCerrarAlternativa3 = KeyCode.Escape;


    private bool estabaVisible;


    void Start()
    {
        if (ventanaPopup == null) return;

        if (EsCuadroDeDialogo())
        {
            Debug.LogWarning("PopupController: la ventana asignada en " + gameObject.name +
                " es el cuadro de dialogo, no la nota de objetivo. Lo maneja DialogoLog, asi que este controlador se apaga.", this);
            enabled = false;
            return;
        }

        ventanaPopup.SetActive(true);
    }


    private bool EsCuadroDeDialogo()
    {
        DialogoLog[] dialogos = FindObjectsByType<DialogoLog>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < dialogos.Length; i++)
        {
            if (dialogos[i].PanelDialogo == ventanaPopup) return true;
        }

        return false;
    }

    void Update()
    {
        bool visible = ventanaPopup != null && ventanaPopup.activeSelf;

        if (visible && (Input.GetKeyDown(teclaCerrar) || Input.GetKeyDown(teclaCerrarAlternativa) || Input.GetKeyDown(teclaCerrarAlternativa2) || Input.GetKeyDown(teclaCerrarAlternativa3) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            CerrarVentana();
            return;
        }

        if (estabaVisible && !visible)
        {
            estabaVisible = false;
            AvisarCierre();
            return;
        }

        estabaVisible = visible;
    }

    public void AbrirVentana(bool esInicial = false)
    {
        esPopupInicial = esInicial;
        if (ventanaPopup != null)
        {
            ventanaPopup.SetActive(true);
        }
    }

    public void CerrarVentana()
    {
        if (ventanaPopup != null && ventanaPopup.activeSelf)
        {
            ventanaPopup.SetActive(false);
            estabaVisible = false;

            AvisarCierre();
        }
    }

    private void AvisarCierre()
    {
        Debug.Log("Ventana de objetivos cerrada.");

        OnPopupClosed?.Invoke();
    }

    // Método de apoyo para consultar el estado visible de la ventana
    public bool EstaVisible()
    {
        return ventanaPopup != null && ventanaPopup.activeSelf;
    }
}
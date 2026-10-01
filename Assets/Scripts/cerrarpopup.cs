using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    [SerializeField] private GameObject ventanaPopup;

    public event Action OnPopupClosed;

    public bool esPopupInicial { get; set; } = true;

    public GameObject Ventana { get { return ventanaPopup; } }

    [Header("Tecla para cerrar la nota")]
    public KeyCode teclaCerrar = KeyCode.Return;


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
        if (ventanaPopup != null && ventanaPopup.activeSelf)
        {
            if (Input.GetKeyDown(teclaCerrar) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                CerrarVentana();
            }
        }
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
            Debug.Log("Ventana de objetivos cerrada.");

            OnPopupClosed?.Invoke();
        }
    }

    // Método de apoyo para consultar el estado visible de la ventana
    public bool EstaVisible()
    {
        return ventanaPopup != null && ventanaPopup.activeSelf;
    }
}
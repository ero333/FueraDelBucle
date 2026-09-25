using System;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    [SerializeField] private GameObject ventanaPopup;

    public event Action OnPopupClosed;

    public bool esPopupInicial { get; set; } = true;

    void Start()
    {
        if (ventanaPopup != null)
        {
            ventanaPopup.SetActive(true);
        }
    }

    void Update()
    {
        if (ventanaPopup != null && ventanaPopup.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.X) ||
                Input.GetKeyDown(KeyCode.E) ||
                Input.GetKeyDown(KeyCode.Return) ||
                Input.GetKeyDown(KeyCode.KeypadEnter))
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
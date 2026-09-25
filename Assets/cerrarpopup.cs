using UnityEngine;

public class PopupController : MonoBehaviour
{
    [SerializeField] private GameObject ventanaPopup; // referencia al objeto de la ventana

    void Start()
    {
        // Aseguramos que la ventana esté activa al inicio (puedes cambiarlo según tu lógica)
        ventanaPopup.SetActive(true);
    }

    void Update()
    {
        // Detecta si el jugador presiona X, E o Enter
        if (ventanaPopup.activeSelf &&
            (Input.GetKeyDown(KeyCode.X) ||
             Input.GetKeyDown(KeyCode.E) ||
             Input.GetKeyDown(KeyCode.Return)))
        {
            CerrarVentana();
        }
    }

    private void CerrarVentana()
    {
        ventanaPopup.SetActive(false);
        Debug.Log("Ventana cerrada con teclado.");
    }
}

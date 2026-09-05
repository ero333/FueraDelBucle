using UnityEngine;


public class BotonVentana : MonoBehaviour
{
    [SerializeField] private GameObject ventana;

    void Start()
    {
        ventana.SetActive(false);
    }

    public void AbrirVentana()
    {
        ventana.SetActive(true);
    }

    public void CerrarVentana()
    {
        ventana.SetActive(false);
    }
}



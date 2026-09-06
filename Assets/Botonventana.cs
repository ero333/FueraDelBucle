using UnityEngine;


public class BotonVentana : MonoBehaviour
{
    [SerializeField] private GameObject ventana;

    void Start()
    {
        if (ventana == null)
        {
            Debug.LogWarning("BotonVentana: falta asignar la ventana en el Inspector de " + gameObject.name, this);
            return;
        }

        ventana.SetActive(false);
    }

    public void AbrirVentana()
    {
        if (ventana == null) return;

        ventana.SetActive(true);
    }

    public void CerrarVentana()
    {
        if (ventana == null) return;

        ventana.SetActive(false);
    }
}

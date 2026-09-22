using UnityEngine;

public class BotonVentana : MonoBehaviour
{
    [Header("Configuración de Ventana")]
    public GameObject ventana;

    // Función para abrir o mostrar la ventana asignada
    public void AbrirVentana()
    {
        if (ventana != null)
        {
            ventana.SetActive(true);
        }
        else
        {
            Debug.LogWarning("BotonVentana: falta asignar la ventana en el Inspector de Boton");
        }
    }

    // Función para cerrar u ocultar la ventana asignada
    public void CerrarVentana()
    {
        if (ventana != null)
        {
            ventana.SetActive(false);
        }
        else
        {
            Debug.LogWarning("BotonVentana: falta asignar la ventana en el Inspector de Boton");
        }
    }

    // Función alternable (toggle)
    public void AlternarVentana()
    {
        if (ventana != null)
        {
            ventana.SetActive(!ventana.activeSelf);
        }
        else
        {
            Debug.LogWarning("BotonVentana: falta asignar la ventana en el Inspector de Boton");
        }
    }
}
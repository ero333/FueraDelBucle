using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LuXVisibilidad : MonoBehaviour
{
    private Light2D luzGlobal;
    private Light2D luzJugador;
    public string tagGlobal = "Oscuridad";
    public string tagJugador = "IVisibilidadJugador";

    [Header("Configuración de Visibilidad")]
    public float intensidadNormal = 1f;
    public float intensidadOscuridad = 0.02f;
    public float Transicion = 2f;

    private bool oscuridadActiva = false;
    private float intensidadObjetivo;

    void Start()
    {
        GameObject LuzGlobal = GameObject.FindGameObjectWithTag("Oscuridad");
        if (LuzGlobal != null)
        {
            luzGlobal = LuzGlobal.GetComponent<Light2D>();
            luzGlobal.intensity = intensidadNormal;
            intensidadObjetivo = intensidadNormal;
        }


        GameObject objJugador = GameObject.FindGameObjectWithTag("IVisibilidadJugador");
        if (objJugador != null)
        {
            luzJugador = objJugador.GetComponent<Light2D>();
            luzJugador.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("l"))
        {
            AlternarOscuridad();
        }

        if (luzGlobal != null)
        {
            luzGlobal.intensity = Mathf.Lerp(luzGlobal.intensity,
            intensidadObjetivo, Time.deltaTime * Transicion);
        }
    }

    public void AlternarOscuridad()
    {
        oscuridadActiva = !oscuridadActiva;
        if (oscuridadActiva)
        {
            intensidadObjetivo = intensidadOscuridad;
            if (luzJugador != null) luzJugador.enabled = true;
        }
        else
        {
            intensidadObjetivo = intensidadNormal;
            if (luzJugador != null) luzJugador.enabled = false;
        }
    }
}

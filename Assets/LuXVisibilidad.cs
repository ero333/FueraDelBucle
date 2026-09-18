using UnityEngine;
using System.Collections;
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

    [Header("Entrada")]
    // A N I M A C I O N
    public Animator anim;
    public string triggerAnim = "Apagado";

    // T I E M P O   D E   E S P E R A  P A R A   A P G A R   L U C E S
    public bool Inicio = true;
    public float Espera = 1f;
    private bool Activo = false;

    // T R I G G E R 
    public bool usarTrigger = false;
    public string tagTriggerJugador = "Player";
    public string tagLux = "Lux";



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

        // A N I M A C I O N
        GameObject objLux = GameObject.FindGameObjectWithTag(tagLux);
        if (objLux != null)
        {
            anim = objLux.GetComponent<Animator>();
        }

        // T I E M P O   D E   E S P E R A  P A R A   A P G A R   L U C E S

        if (Inicio && !usarTrigger)
        {
            StartCoroutine(Entrada());
        }
    }

    void Update()
    {

        if (luzGlobal != null)
        {
            luzGlobal.intensity = Mathf.Lerp(luzGlobal.intensity,
            intensidadObjetivo, Time.deltaTime * Transicion);
        } 
    }

    // T I E M P O   D E   E S P E R A  P A R A   A P G A R   L U C E S

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (usarTrigger && !Activo && collision.CompareTag(tagTriggerJugador))
        {
            Activo = true;
            StartCoroutine(Entrada());
        }
    }

    private IEnumerator Entrada()
    {
        yield return new WaitForSeconds(Espera);

        if (anim != null)
        {
            anim.SetTrigger(triggerAnim);
        }

        AlternarOscuridad();
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

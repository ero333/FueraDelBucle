using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Calificacion : MonoBehaviour
{
    public const string ClaveHistoria = "CalificacionHistoria";
    public const string ClaveArte = "CalificacionArte";
    public const string ClaveJugabilidad = "CalificacionJugabilidad";
    public const string ClaveEnviada = "CalificacionEnviada";

    [Header("Estrellas de cada categoria, de izquierda a derecha")]
    public Image[] estrellasHistoria;
    public Image[] estrellasArte;
    public Image[] estrellasJugabilidad;

    [Header("Colores de las estrellas")]
    public Color colorLlena = new Color(1f, 0.72f, 0.15f);
    public Color colorVacia = new Color(0.78f, 0.84f, 0.9f);

    [Header("Que se muestra al enviar")]
    public GameObject botonEnviar;
    public GameObject textoGracias;

    [Header("Que pasa al cerrar")]
    [Tooltip("Vacio: la ventana se oculta. Con el nombre de una escena: la carga.")]
    public string escenaAlCerrar = "";

    private int historia;
    private int arte;
    private int jugabilidad;

    private bool yaEnviada;
    private int historiaEnviada;
    private int arteEnviada;
    private int jugabilidadEnviada;

    void OnEnable()
    {
        historia = PlayerPrefs.GetInt(ClaveHistoria, 0);
        arte = PlayerPrefs.GetInt(ClaveArte, 0);
        jugabilidad = PlayerPrefs.GetInt(ClaveJugabilidad, 0);
        yaEnviada = PlayerPrefs.GetInt(ClaveEnviada, 0) == 1;

        historiaEnviada = historia;
        arteEnviada = arte;
        jugabilidadEnviada = jugabilidad;

        Pintar();
        ActualizarBoton();
    }

    public void PonerHistoria(int puntaje)
    {
        historia = Alternar(historia, puntaje);
        Pintar();
        ActualizarBoton();
    }

    public void PonerArte(int puntaje)
    {
        arte = Alternar(arte, puntaje);
        Pintar();
        ActualizarBoton();
    }

    public void PonerJugabilidad(int puntaje)
    {
        jugabilidad = Alternar(jugabilidad, puntaje);
        Pintar();
        ActualizarBoton();
    }

    public void BorrarHistoria()
    {
        historia = 0;
        Pintar();
        ActualizarBoton();
    }

    public void BorrarArte()
    {
        arte = 0;
        Pintar();
        ActualizarBoton();
    }

    public void BorrarJugabilidad()
    {
        jugabilidad = 0;
        Pintar();
        ActualizarBoton();
    }

    public void BorrarTodo()
    {
        historia = 0;
        arte = 0;
        jugabilidad = 0;
        Pintar();
        ActualizarBoton();
    }

    public void Enviar()
    {
        PlayerPrefs.SetInt(ClaveHistoria, historia);
        PlayerPrefs.SetInt(ClaveArte, arte);
        PlayerPrefs.SetInt(ClaveJugabilidad, jugabilidad);
        PlayerPrefs.SetInt(ClaveEnviada, 1);
        PlayerPrefs.Save();

        yaEnviada = true;
        historiaEnviada = historia;
        arteEnviada = arte;
        jugabilidadEnviada = jugabilidad;

        ActualizarBoton();
    }

    public void Cerrar()
    {
        if (string.IsNullOrEmpty(escenaAlCerrar))
        {
            gameObject.SetActive(false);
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaAlCerrar);
    }

    private int Alternar(int actual, int puntaje)
    {
        int nuevo = Mathf.Clamp(puntaje, 0, 5);
        return (nuevo == actual) ? 0 : nuevo;
    }

    private bool HayCambio()
    {
        return historia != historiaEnviada
            || arte != arteEnviada
            || jugabilidad != jugabilidadEnviada;
    }

    private void ActualizarBoton()
    {
        bool mostrarEnviar = !yaEnviada || HayCambio();

        if (botonEnviar != null)
        {
            botonEnviar.SetActive(mostrarEnviar);
        }

        if (textoGracias != null)
        {
            textoGracias.SetActive(yaEnviada && !mostrarEnviar);
        }
    }

    private void Pintar()
    {
        PintarFila(estrellasHistoria, historia);
        PintarFila(estrellasArte, arte);
        PintarFila(estrellasJugabilidad, jugabilidad);
    }

    private void PintarFila(Image[] fila, int puntaje)
    {
        if (fila == null) return;

        for (int i = 0; i < fila.Length; i++)
        {
            if (fila[i] == null) continue;

            fila[i].color = (i < puntaje) ? colorLlena : colorVacia;
        }
    }
}

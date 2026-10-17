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

    void OnEnable()
    {
        historia = PlayerPrefs.GetInt(ClaveHistoria, 0);
        arte = PlayerPrefs.GetInt(ClaveArte, 0);
        jugabilidad = PlayerPrefs.GetInt(ClaveJugabilidad, 0);

        if (botonEnviar != null)
        {
            botonEnviar.SetActive(true);
        }

        if (textoGracias != null)
        {
            textoGracias.SetActive(false);
        }

        Pintar();
    }

    public void PonerHistoria(int puntaje)
    {
        historia = Mathf.Clamp(puntaje, 0, 5);
        Pintar();
    }

    public void PonerArte(int puntaje)
    {
        arte = Mathf.Clamp(puntaje, 0, 5);
        Pintar();
    }

    public void PonerJugabilidad(int puntaje)
    {
        jugabilidad = Mathf.Clamp(puntaje, 0, 5);
        Pintar();
    }

    public void Enviar()
    {
        if (historia == 0 && arte == 0 && jugabilidad == 0) return;

        PlayerPrefs.SetInt(ClaveHistoria, historia);
        PlayerPrefs.SetInt(ClaveArte, arte);
        PlayerPrefs.SetInt(ClaveJugabilidad, jugabilidad);
        PlayerPrefs.SetInt(ClaveEnviada, 1);
        PlayerPrefs.Save();

        if (botonEnviar != null)
        {
            botonEnviar.SetActive(false);
        }

        if (textoGracias != null)
        {
            textoGracias.SetActive(true);
        }
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

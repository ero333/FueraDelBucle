using UnityEngine;

[CreateAssetMenu(fileName ="NPCDialogoNuevo", menuName ="LOG Dialogo")]
public class NPCDialogos : ScriptableObject
{
    public string NombreNPC;
    public Sprite LogRetrato;

    [Tooltip("Retrato distinto para alguna linea puntual. Lo que quede vacio usa el retrato de arriba.")]
    public Sprite[] retratosPorLinea;

    [Tooltip("Tamano del retrato en esa linea. 1 es el normal, 1.2 lo agranda un 20 por ciento. En 0 queda el normal.")]
    public float[] escalaRetratoPorLinea;

    [Tooltip("Cuanto se corre el retrato en esa linea. Positivo lo lleva a la derecha, negativo a la izquierda.")]
    public float[] desplazamientoRetratoPorLinea;

    public string[] lineasDialogo;
    public bool[] autoProgresLineas;
    public float autoProgresDelay = 1.5f;
    public float velocidadTypeo;
    public AudioClip sonidoVoz;
    public float pitchVoz = 1f;
    
    


}

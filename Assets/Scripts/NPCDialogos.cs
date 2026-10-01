using UnityEngine;

[CreateAssetMenu(fileName ="NPCDialogoNuevo", menuName ="LOG Dialogo")]
public class NPCDialogos : ScriptableObject
{
    public string NombreNPC;
    public Sprite LogRetrato;
    public string[] lineasDialogo;
    public bool[] autoProgresLineas;
    public float autoProgresDelay = 1.5f;
    public float velocidadTypeo;
    public AudioClip sonidoVoz;
    public float pitchVoz = 1f;
    
    


}

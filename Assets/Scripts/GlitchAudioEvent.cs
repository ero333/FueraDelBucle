using UnityEngine;

public class GlitchAudioEvent : MonoBehaviour
{
    public void ReproducirGlitch()
    {
        if (SonidoBotones.Instancia != null)
        {
            SonidoBotones.Instancia.ReproducirGlitch();
        }
    }
}
using UnityEngine;

// Va en el LUX. El sonido lo dispara un evento de la animación Apagado, así suena sólo cuando LUX
// está en la escena y de verdad hace su animación, y en el frame que corresponde.
public class SonidoLux : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip sonidoApagado;

    public void SonarApagado()
    {
        Volumen.SonarEfecto(sonidoApagado, 1f);
    }
}

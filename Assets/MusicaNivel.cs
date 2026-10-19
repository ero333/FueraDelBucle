using UnityEngine;

public class MusicaNivel : MonoBehaviour
{
    [SerializeField] private AudioClip cancionDelNivel;

    private void OnEnable()
    {
        Volumen.Cambio += AplicarVolumen;
    }

    private void OnDisable()
    {
        Volumen.Cambio -= AplicarVolumen;
    }

    private void AplicarVolumen()
    {
        AudioSource fuente = GetComponent<AudioSource>();

        if (fuente != null)
        {
            fuente.mute = Volumen.Silenciado;
            fuente.volume = Volumen.Musica;
        }
    }

    private void Start()
    {
        AudioSource localSource = GetComponent<AudioSource>();

        
        if (MusicManager.Instance != null)
        {
          
            if (localSource != null)
            {
                localSource.Stop();
            }

            
            AudioSource globalSource = MusicManager.Instance.GetComponent<AudioSource>();
            if (globalSource != null && cancionDelNivel != null)
            {
                if (globalSource.clip != cancionDelNivel)
                {
                    globalSource.clip = cancionDelNivel;
                    globalSource.Play();
                }
            }
        }
        else
        {
           
            if (localSource != null && cancionDelNivel != null)
            {
                localSource.clip = cancionDelNivel;
                localSource.loop = true;

               
                localSource.mute = Volumen.Silenciado;
                localSource.volume = Volumen.Musica;

                localSource.Play();
            }
        }
    }
}


using UnityEngine;

public class MusicaNivel : MonoBehaviour
{
    [SerializeField] private AudioClip cancionDelNivel;

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

               
                bool estaMuteado = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
                localSource.mute = estaMuteado;

                localSource.Play();
            }
        }
    }
}


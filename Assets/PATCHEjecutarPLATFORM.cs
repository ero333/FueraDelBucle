using UnityEngine;
public class PATCHEjecutarPlatform : MonoBehaviour

{
    

    public PATCHPlatformManager RomperPlatform;

    public GameObject enemigoAActivar;

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        

        if (collision.CompareTag("Player"))
        {
            

            if (enemigoAActivar != null)
            {
                enemigoAActivar.SetActive(true);

                Animator anim = enemigoAActivar.GetComponent<Animator>();
                if(anim != null)
                {
                    anim.SetTrigger("ataque");
                }

            }

            
            if (RomperPlatform != null)
            {
                RomperPlatform.RomperSiguienteTanda();
            }
            else
            {
                Debug.LogError("Falta asignar la referencia de RomperPlatform en el Inspector.");
            }

            
            gameObject.SetActive(false);
        }
    }
   



}


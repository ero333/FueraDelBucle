using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PATCHPlatformManager : MonoBehaviour
{
    [System.Serializable]
    public class PlatformTanda
    {
        public string nombreTanda = "Tanda X";
        public List<GameObject> plataformasARomper; 
    }

    [Header("Configuración de Tandas")]
    public List<PlatformTanda> tandasDePlataformas;
    private int indiceTandaActual = 0;

    private Animator anim;

    /// <summary>
    /// Se llama cada vez que el enemigo aparece/se activa.
    /// </summary>
    /// 
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void RomperSiguienteTanda()
    {
        {
            if (indiceTandaActual < tandasDePlataformas.Count)
            {
                PlatformTanda tanda = tandasDePlataformas[indiceTandaActual];

                StartCoroutine(DestruirPlataforma(tanda));

                indiceTandaActual++; 
            }
            else
            {
                Debug.LogWarning("Se alcanzaron todas las tandas de plataformas a romper.");
            }
        }


    }

    IEnumerator DestruirPlataforma(PlatformTanda tanda)
    {
        foreach (GameObject plataforma in tanda.plataformasARomper)
        {
            if (plataforma != null)
            {

                Collider2D col2D = plataforma.GetComponent<Collider2D>();

                anim = plataforma.GetComponent<Animator>();
                if(anim != null)
                {
                    anim.SetTrigger("INDCDESTR");
                    anim.SetTrigger("INDCDESTR-S");
                    anim.SetTrigger("INDCDESTR-L");
                    yield return new WaitForSeconds(0.3f);
                    anim.SetTrigger("DESTRUIR");
                    anim.SetTrigger("DESTRUIR-S");
                    anim.SetTrigger("DESTRUIR-L");
                }
                

            }
        }
        
        yield return new WaitForSeconds(0.7f);
        foreach (GameObject plataforma in tanda.plataformasARomper)
        {
            if(plataforma != null)
            {
                plataforma.SetActive(false);
            }
        }
        
    }
}

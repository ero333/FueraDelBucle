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

    // En qué parte de la animación de rotura la plataforma ya se ve hecha escombros (0 = al empezar, 1 = al terminar).
    private const float FraccionHastaEscombros = 0.75f;

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

    // Apenas la plataforma queda en escombros deja de ser sólida, para que el jugador empiece a caer
    // en ese momento y no recién cuando se desactiva todo el grupo.
    // Solo se apaga el collider si la plataforma de verdad se está rompiendo en pantalla (su sprite
    // cambió). Si todavía no tiene animación de rotura (clips vacíos) se ve entera hasta que se
    // desactiva: ahí el collider se mantiene, así no se atraviesa una plataforma que se ve entera.
    private IEnumerator DesactivarColliderAlHacerseEscombros(Collider2D colision, SpriteRenderer sprite, Sprite spriteOriginal, Animator animador)
    {
        float espera = LargoAnimacionRotura(animador) * FraccionHastaEscombros;

        if (colision == null || sprite == null || espera <= 0f)
            yield break;

        Sprite spriteAlRomper = sprite.sprite;

        yield return new WaitForSeconds(espera);

        if (colision == null || sprite == null)
            yield break;

        bool seEstaRompiendo = sprite.sprite != spriteOriginal && sprite.sprite != spriteAlRomper;

        if (seEstaRompiendo)
        {
            colision.enabled = false;
        }
    }

    // Largo de la animación de rotura de la plataforma: el clip que no es ni el idle ni el indicador.
    private float LargoAnimacionRotura(Animator animador)
    {
        float largo = 0f;

        if (animador != null && animador.runtimeAnimatorController != null)
        {
            foreach (AnimationClip clip in animador.runtimeAnimatorController.animationClips)
            {
                string nombre = clip.name.ToLowerInvariant();

                if (nombre.Contains("idle") || nombre.Contains("indic")) continue;

                largo = Mathf.Max(largo, clip.length);
            }
        }

        return largo;
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
                    // Aspecto original de la plataforma, para saber después si de verdad se está rompiendo.
                    SpriteRenderer spritePlataforma = plataforma.GetComponentInChildren<SpriteRenderer>();
                    Sprite spriteOriginal = spritePlataforma != null ? spritePlataforma.sprite : null;
                    Animator animadorPlataforma = anim;

                    anim.SetTrigger("INDCDESTR");
                    anim.SetTrigger("INDCDESTR-S");
                    anim.SetTrigger("INDCDESTR-L");
                    yield return new WaitForSeconds(0.3f);
                    anim.SetTrigger("DESTRUIR");
                    anim.SetTrigger("DESTRUIR-S");
                    anim.SetTrigger("DESTRUIR-L");

                    StartCoroutine(DesactivarColliderAlHacerseEscombros(col2D, spritePlataforma, spriteOriginal, animadorPlataforma));
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

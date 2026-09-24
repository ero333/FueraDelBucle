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

    /// <summary>
    /// Se llama cada vez que el enemigo aparece/se activa.
    /// </summary>
    public void RomperSiguienteTanda()
    {
        {
            if (indiceTandaActual < tandasDePlataformas.Count)
            {
                PlatformTanda tanda = tandasDePlataformas[indiceTandaActual];

                foreach (GameObject plataforma in tanda.plataformasARomper)
                {
                    if (plataforma != null)
                    {
                        
                        Collider2D col2D = plataforma.GetComponent<Collider2D>();
                        plataforma.SetActive(false);

                    }
                }

                indiceTandaActual++; 
            }
            else
            {
                Debug.LogWarning("Se alcanzaron todas las tandas de plataformas a romper.");
            }
        }


    }
}

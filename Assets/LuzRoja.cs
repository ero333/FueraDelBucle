using UnityEngine;
using System.Collections;

public class LuzRoja : MonoBehaviour
{
    public GameObject luzRoja;
    public float tiempoEntreDisparos = 1.5f;
    public float duracionTitileo = 0.15f;

    private void Start()
    {
        luzRoja.SetActive(false);
        StartCoroutine(Titilar());
    }

    private IEnumerator Titilar()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoEntreDisparos);

            luzRoja.SetActive(true);

            yield return new WaitForSeconds(duracionTitileo);

            luzRoja.SetActive(false);
        }
    }
}
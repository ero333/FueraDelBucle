using UnityEngine;

public class Einteractivo : MonoBehaviour
{
    public GameObject objectToToggle;

    private void Awake()
    {
        if (objectToToggle != null)
        {
            objectToToggle.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (objectToToggle != null)
            {
                objectToToggle.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (objectToToggle != null)
            {
                objectToToggle.SetActive(false);
            }
        }
    }
}
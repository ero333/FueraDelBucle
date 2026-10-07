using UnityEngine;

public class HELPMENU : MonoBehaviour
{
    [Header("Panel del Menú de Ayuda (Sprite / Image)")]
    public GameObject helpPanel; 

    [Header("Tecla de Acceso Rápido")]
    public KeyCode teclaAyuda = KeyCode.H;

    void Awake()
    {
        
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
        }
    }

    void Update()
    {
       
        if (Input.GetKeyDown(teclaAyuda))
        {
            ToggleHelpMenu();
        }
    }

    public void ToggleHelpMenu()
    {
        if (helpPanel == null) return;

        bool estaAbriendo = !helpPanel.activeSelf;
        helpPanel.SetActive(estaAbriendo);

        if (estaAbriendo)
        {
            helpPanel.transform.SetAsLastSibling(); 
            Time.timeScale = 0f;                  
        }
        else
        {
            Time.timeScale = 1f;                 
        }
    }
}
using UnityEngine;

public class HELPMENU : MonoBehaviour
{
    [Header("Panel del Menú de Ayuda (Sprite / Image)")]
    public GameObject helpPanel; 

    [Header("Tecla de Acceso Rápido")]
    public KeyCode teclaAyuda = KeyCode.H;

    private float escalaPrevia = 1f;
    private bool escalaGuardada;

    void Awake()
    {
        
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
        }

        escalaGuardada = false;
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
            escalaPrevia = Time.timeScale;
            escalaGuardada = true;
            Time.timeScale = 0f;
        }
        else
        {
            RestaurarEscala();
        }
    }

    private void RestaurarEscala()
    {
        if (!escalaGuardada) return;

        Time.timeScale = escalaPrevia;
        escalaGuardada = false;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HELPMENU : MonoBehaviour
{
    [Header("Panel del Menú de Ayuda (Sprite / Image)")]
    public GameObject helpPanel;

    [Header("Tecla de Acceso Rápido")]
    public KeyCode teclaAyuda = KeyCode.H;

    [Header("Cerrar al tocar otro boton")]
    [Tooltip("Si esta prendido, tocar cualquier cosa fuera del panel lo cierra.")]
    public bool cerrarAlTocarAfuera = true;

    private float escalaPrevia = 1f;
    private bool escalaGuardada;
    private int frameDeApertura = -1;

    private static readonly List<RaycastResult> resultados = new List<RaycastResult>();

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

    // Mira el boton del mouse al SOLTARLO y desde LateUpdate, porque Unity dispara el
    // onClick recien al soltar y dentro de su propio Update. Si se mirara al apretar,
    // el panel se cerraria antes de que corriera el onClick del boton de ayuda y ese
    // mismo clic lo volveria a abrir.
    void LateUpdate()
    {
        if (!cerrarAlTocarAfuera) return;
        if (helpPanel == null || !helpPanel.activeSelf) return;
        if (Time.frameCount == frameDeApertura) return;
        if (!Input.GetMouseButtonUp(0)) return;
        if (ElToqueCayoEnElPanel()) return;

        CerrarHelpMenu();
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
            frameDeApertura = Time.frameCount;
        }
        else
        {
            RestaurarEscala();
        }
    }

    public void CerrarHelpMenu()
    {
        if (helpPanel == null || !helpPanel.activeSelf) return;

        helpPanel.SetActive(false);
        RestaurarEscala();
    }

    private bool ElToqueCayoEnElPanel()
    {
        if (EventSystem.current == null) return false;

        PointerEventData datos = new PointerEventData(EventSystem.current);
        datos.position = Input.mousePosition;

        resultados.Clear();
        EventSystem.current.RaycastAll(datos, resultados);

        for (int i = 0; i < resultados.Count; i++)
        {
            GameObject tocado = resultados[i].gameObject;

            if (tocado != null && tocado.transform.IsChildOf(helpPanel.transform)) return true;
        }

        return false;
    }

    private void RestaurarEscala()
    {
        if (!escalaGuardada) return;

        Time.timeScale = escalaPrevia;
        escalaGuardada = false;
    }
}

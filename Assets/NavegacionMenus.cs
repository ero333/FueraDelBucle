using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavegacionMenus : MonoBehaviour
{
    public static string[] escenasDeMenu = { "MenuPrincipal", "MapaNiveles" };
    public static Color colorSeleccionado = new Color(0.45f, 0.85f, 1f, 1f);

    private readonly HashSet<Selectable> yaResaltados = new HashSet<Selectable>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Crear()
    {
        if (FindAnyObjectByType<NavegacionMenus>() != null) return;

        GameObject objeto = new GameObject("NavegacionMenus");
        objeto.AddComponent<NavegacionMenus>();
        DontDestroyOnLoad(objeto);
    }

    private void Update()
    {
        EventSystem eventos = EventSystem.current;
        if (eventos == null) return;

        if (!HayMenuAbierto())
        {
            if (eventos.currentSelectedGameObject != null)
            {
                eventos.SetSelectedGameObject(null);
            }
            return;
        }

        AplicarResaltado();

        GameObject actual = eventos.currentSelectedGameObject;
        if (EsUsable(actual)) return;

        GameObject primero = BuscarPrimero();
        if (primero != null)
        {
            eventos.SetSelectedGameObject(primero);
        }
    }

    private void AplicarResaltado()
    {
        Selectable[] todos = Selectable.allSelectablesArray;

        for (int i = 0; i < todos.Length; i++)
        {
            Selectable actual = todos[i];
            if (actual == null || yaResaltados.Contains(actual)) continue;

            yaResaltados.Add(actual);

            if (actual.transition != Selectable.Transition.ColorTint) continue;

            ColorBlock colores = actual.colors;
            colores.highlightedColor = colorSeleccionado;
            colores.selectedColor = colorSeleccionado;
            actual.colors = colores;
        }
    }

    private bool HayMenuAbierto()
    {
        if (HayDialogoVisible()) return false;

        if (Time.timeScale == 0f) return true;

        string escena = SceneManager.GetActiveScene().name;
        for (int i = 0; i < escenasDeMenu.Length; i++)
        {
            if (escena == escenasDeMenu[i]) return true;
        }

        return false;
    }

    private bool HayDialogoVisible()
    {
        DialogoLog[] dialogos = FindObjectsByType<DialogoLog>(FindObjectsSortMode.None);

        for (int i = 0; i < dialogos.Length; i++)
        {
            GameObject panel = dialogos[i].PanelDialogo;
            if (panel == null || !panel.activeInHierarchy) continue;

            CanvasGroup grupo = panel.GetComponent<CanvasGroup>();
            if (grupo != null && grupo.alpha <= 0f) continue;

            return true;
        }

        return false;
    }

    private bool EsUsable(GameObject objeto)
    {
        if (objeto == null || !objeto.activeInHierarchy) return false;

        Selectable seleccionable = objeto.GetComponent<Selectable>();
        if (seleccionable == null || !seleccionable.IsInteractable()) return false;

        return EsVisible(seleccionable);
    }

    private bool EsVisible(Selectable seleccionable)
    {
        CanvasGroup[] grupos = seleccionable.GetComponentsInParent<CanvasGroup>(false);

        for (int i = 0; i < grupos.Length; i++)
        {
            if (grupos[i].alpha <= 0f || !grupos[i].interactable) return false;
        }

        return true;
    }

    private GameObject BuscarPrimero()
    {
        Selectable[] todos = Selectable.allSelectablesArray;

        Selectable elegido = null;
        int mejorCanvas = int.MinValue;
        float mejorAltura = float.MinValue;

        for (int i = 0; i < todos.Length; i++)
        {
            Selectable actual = todos[i];
            if (actual == null || !actual.gameObject.activeInHierarchy) continue;
            if (!actual.IsInteractable() || !EsVisible(actual)) continue;

            Canvas canvas = actual.GetComponentInParent<Canvas>();
            int orden = canvas != null ? canvas.sortingOrder : 0;
            float altura = actual.transform.position.y;

            if (orden > mejorCanvas || (orden == mejorCanvas && altura > mejorAltura))
            {
                elegido = actual;
                mejorCanvas = orden;
                mejorAltura = altura;
            }
        }

        return elegido != null ? elegido.gameObject : null;
    }
}

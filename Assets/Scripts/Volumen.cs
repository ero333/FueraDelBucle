using UnityEngine;

public static class Volumen
{
    public const string ClaveMusica = "VolumenMusica";
    public const string ClaveEfectos = "VolumenEfectos";

    public static event System.Action Cambio;

    private static bool cargado;
    private static float musica = 1f;
    private static float efectos = 1f;

    public static float Musica
    {
        get
        {
            Cargar();
            return musica;
        }
    }

    public static float Efectos
    {
        get
        {
            Cargar();
            return efectos;
        }
    }

    public static void PonerMusica(float valor)
    {
        Cargar();
        musica = Mathf.Clamp01(valor);
        PlayerPrefs.SetFloat(ClaveMusica, musica);
        PlayerPrefs.Save();
        Avisar();
    }

    public static void PonerEfectos(float valor)
    {
        Cargar();
        efectos = Mathf.Clamp01(valor);
        PlayerPrefs.SetFloat(ClaveEfectos, efectos);
        PlayerPrefs.Save();
        Avisar();
    }

    public static float DeEfecto(float volumenBase)
    {
        return Mathf.Clamp01(volumenBase) * Efectos;
    }

    private static void Cargar()
    {
        if (cargado) return;

        musica = Mathf.Clamp01(PlayerPrefs.GetFloat(ClaveMusica, 1f));
        efectos = Mathf.Clamp01(PlayerPrefs.GetFloat(ClaveEfectos, 1f));
        cargado = true;
    }

    private static void Avisar()
    {
        System.Action aviso = Cambio;

        if (aviso != null)
        {
            aviso();
        }
    }
}

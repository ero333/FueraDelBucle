using UnityEngine;

public static class Volumen
{
    public const string ClaveMusica = "VolumenMusica";
    public const string ClaveEfectos = "VolumenEfectos";
    public const string ClaveSilencio = "MusicMuted";

    public static event System.Action Cambio;

    private static bool cargado;
    private static float musica = 1f;
    private static float efectos = 1f;
    private static bool silenciado;

    public static bool Silenciado
    {
        get
        {
            Cargar();
            return silenciado;
        }
    }

    public static float Musica
    {
        get
        {
            Cargar();
            return silenciado ? 0f : musica;
        }
    }

    public static float Efectos
    {
        get
        {
            Cargar();
            return silenciado ? 0f : efectos;
        }
    }

    public static float MusicaGuardada
    {
        get
        {
            Cargar();
            return musica;
        }
    }

    public static float EfectosGuardados
    {
        get
        {
            Cargar();
            return efectos;
        }
    }

    public static void PonerSilencio(bool valor)
    {
        Cargar();

        if (silenciado == valor) return;

        silenciado = valor;
        PlayerPrefs.SetInt(ClaveSilencio, silenciado ? 1 : 0);
        PlayerPrefs.Save();
        AplicarCorteGeneral();
        Avisar();
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

    public static void SonarEfecto(AudioClip clip, float volumenBase)
    {
        if (clip == null) return;

        float volumen = DeEfecto(volumenBase);

        if (volumen <= 0f) return;

        GameObject objeto = new GameObject("SFX " + clip.name);
        AudioSource fuente = objeto.AddComponent<AudioSource>();

        fuente.clip = clip;
        fuente.volume = volumen;
        fuente.spatialBlend = 0f;
        fuente.playOnAwake = false;
        fuente.Play();

        Object.Destroy(objeto, clip.length + 0.1f);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AlArrancar()
    {
        Cargar();
        AplicarCorteGeneral();
    }

    private static void AplicarCorteGeneral()
    {
        AudioListener.volume = silenciado ? 0f : 1f;
    }

    private static void Cargar()
    {
        if (cargado) return;

        musica = Mathf.Clamp01(PlayerPrefs.GetFloat(ClaveMusica, 1f));
        efectos = Mathf.Clamp01(PlayerPrefs.GetFloat(ClaveEfectos, 1f));
        silenciado = PlayerPrefs.GetInt(ClaveSilencio, 0) == 1;
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

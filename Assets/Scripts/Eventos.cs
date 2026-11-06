using System;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;
using AnalyticsEvent = Unity.Services.Analytics.Event;

public class Eventos : MonoBehaviour
{
    public static Eventos Singleton;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
    }

    public void EventoLevel_Start(int numeroNivelActual)
    {
        CustomEvent evento = new CustomEvent("nombre_del_evento")
        {
            { "nombre_del_parametro", numeroNivelActual }
        };
        AnalyticsService.Instance.RecordEvent(evento);
    }

    public class LevelStartEvent : AnalyticsEvent
    {
        public LevelStartEvent() : base("LevelStart") { }

        public int level { set { SetParameter("level", value); } }
        public bool music { set { SetParameter("music", value); } }
        public bool sound { set { SetParameter("sound", value); } }
    }

    public class LevelCompleteEvent : AnalyticsEvent
    {
        public LevelCompleteEvent() : base("LevelComplete") { }

        public int level { set { SetParameter("level", value); } }
        public int time { set { SetParameter("time", value); } }
        public int phaseAvoid { set { SetParameter("phaseAvoid", value); } }
        public int dash { set { SetParameter("dash", value); } }
        public int pushRpt { set { SetParameter("pushRpt", value); } }
        public int killRpt { set { SetParameter("killRpt", value); } }
        public bool music { set { SetParameter("music", value); } }
        public bool sound { set { SetParameter("sound", value); } }
    }

    public class GameOverEvent : AnalyticsEvent
    {
        public GameOverEvent() : base("GameOver") { }

        public int level { set { SetParameter("level", value); } }
        public int time { set { SetParameter("time", value); } }
        public int phaseAvoid { set { SetParameter("phaseAvoid", value); } }
        public int cordX { set { SetParameter("cordX", value); } }
        public int cordY { set { SetParameter("cordY", value); } }
        public string killer { set { SetParameter("killer", value); } }
        public int pushRpt { set { SetParameter("pushRpt", value); } }
        public int killRpt { set { SetParameter("killRpt", value); } }
        public bool music { set { SetParameter("music", value); } }
        public bool sound { set { SetParameter("sound", value); } }
    }

    public const string KillerVacio = "vacio";
    public const string KillerPatch = "patch";
    public const string KillerRepeater1 = "rpt1";
    public const string KillerRepeater2 = "rpt2";
    public const string KillerSentinel1 = "sen1";
    public const string KillerSentinel2 = "sen2";

    public const int SinNivel = 0;

    public static int NivelActual()
    {
        return NivelDeEscena(SceneManager.GetActiveScene().name);
    }

    public static int NivelDeEscena(string nombreEscena)
    {
        const string prefijo = "Nivel";

        if (string.IsNullOrEmpty(nombreEscena) || !nombreEscena.StartsWith(prefijo)) return SinNivel;

        int numero;

        if (!int.TryParse(nombreEscena.Substring(prefijo.Length), out numero)) return SinNivel;
        if (numero < 1 || numero > 10) return SinNivel;

        return numero;
    }

    public static bool MusicaSonando()
    {
        return Volumen.Musica > 0f;
    }

    public static bool EfectosSonando()
    {
        return Volumen.Efectos > 0f;
    }

    private static string Booleano(bool valor)
    {
        return valor ? "true" : "false";
    }

    public static int TiempoDelNivel()
    {
        return Mathf.RoundToInt(Time.timeSinceLevelLoad);
    }

    public static int ColumnaDeMuerte(float x)
    {
        return Mathf.Clamp(Mathf.RoundToInt(x / 30f), 0, 30);
    }

    public static int FilaDeMuerte(float y)
    {
        return Mathf.Clamp(Mathf.RoundToInt(y / 10f), 0, 10);
    }

    private static int repeatersDelNivel;
    private static int repeatersEmpujados;
    private static int repeatersPisados;
    private static int dashesDelNivel;
    private static int evitadosConPhase;

    private static string ultimoKiller = "enemigo";

    public static string killerEnemigo()
    {
        return ultimoKiller;
    }

    public static void SetKiller(string killer)
    {
        ultimoKiller = killer;
    }

    public static void ContarRepeater()
    {
        repeatersDelNivel++;
    }

    public static void ContarRepeaterEmpujado()
    {
        repeatersEmpujados++;

        Debug.Log("[Analytics] (aviso) Repeater al vacio | " + repeatersEmpujados
            + " de " + repeatersDelNivel + " | pushRpt=" + PorcentajeEmpujados());
    }

    public static int PorcentajeEmpujados()
    {
        if (repeatersDelNivel <= 0) return 0;

        return Mathf.RoundToInt(100f * repeatersEmpujados / repeatersDelNivel);
    }

    public static void ContarRepeaterPisado()
    {
        repeatersPisados++;

        Debug.Log("[Analytics] (aviso) Repeater pisado | " + repeatersPisados
            + " de " + repeatersDelNivel + " | killRpt=" + PorcentajePisados());
    }

    public static void ContarDash()
    {
        dashesDelNivel++;
    }

    public static void ContarPhaseEvito()
    {
        evitadosConPhase++;

        Debug.Log("[Analytics] (aviso) Phase evito dano | phaseAvoid=" + evitadosConPhase);
    }

    public static int PorcentajePisados()
    {
        if (repeatersDelNivel <= 0) return 0;

        return Mathf.RoundToInt(100f * repeatersPisados / repeatersDelNivel);
    }

    public static int DashesDelNivel()
    {
        return dashesDelNivel;
    }

    public static int PhaseAvoid()
    {
        return evitadosConPhase;
    }

    private static void ReiniciarContadores()
    {
        repeatersDelNivel = 0;
        repeatersEmpujados = 0;
        repeatersPisados = 0;
        dashesDelNivel = 0;
        evitadosConPhase = 0;
    }

    private static void Enviar(AnalyticsEvent evento)
    {
        try
        {
            AnalyticsService.Instance.RecordEvent(evento);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[Analytics] No se pudo enviar el evento: " + e.Message);
        }
    }

    public static void LogLevelStart(int level)
    {
        if (level == SinNivel) return;

        LevelStartEvent levelStart = new LevelStartEvent
        {
            level = level,
            music = MusicaSonando(),
            sound = EfectosSonando()
        };

        Debug.Log("[Analytics] LevelStart | level=" + level
        + " | music=" + Booleano(MusicaSonando())
        + " | sound=" + Booleano(EfectosSonando()));

        Enviar(levelStart);
    }

    public static void LogLevelComplete(int level)
    {
        if (level == SinNivel) return;

        LevelCompleteEvent levelComplete = new LevelCompleteEvent
        {
            level = level,
            time = TiempoDelNivel(),
            phaseAvoid = PhaseAvoid(),
            dash = DashesDelNivel(),
            pushRpt = PorcentajeEmpujados(),
            killRpt = PorcentajePisados(),
            music = MusicaSonando(),
            sound = EfectosSonando()
        };

        Debug.Log("[Analytics] LevelComplete | level=" + level
        + " | music=" + Booleano(MusicaSonando())
        + " | sound=" + Booleano(EfectosSonando())
        + " | time=" + TiempoDelNivel()
        + " | phaseAvoid=" + PhaseAvoid()
        + " | dash=" + DashesDelNivel()
        + " | pushRpt=" + PorcentajeEmpujados()
        + " | killRpt=" + PorcentajePisados());

        Enviar(levelComplete);
    }

    public static void LogGameOver(int level, float xJugador, string killer = "enemigo")
    {
        if (level == SinNivel) return;

        SetKiller(killer);

        Debug.Log("[Analytics] GameOver | level=" + level
             + " | music=" + Booleano(MusicaSonando())
             + " | time=" + TiempoDelNivel()
             + " | killer=" + ultimoKiller
             + " | phaseAvoid=" + PhaseAvoid()
             + " | cordX=" + ColumnaDeMuerte(xJugador)
             + " | pushRpt=" + PorcentajeEmpujados()
             + " | killRpt=" + PorcentajePisados());
    }

    public static void LogGameOver(int level, float xJugador, float yJugador, string killer)
    {
        if (level == SinNivel) return;

        GameOverEvent gameOver = new GameOverEvent
        {
            level = level,
            time = TiempoDelNivel(),
            phaseAvoid = PhaseAvoid(),
            cordX = ColumnaDeMuerte(xJugador),
            cordY = FilaDeMuerte(yJugador),
            killer = killer,
            pushRpt = PorcentajeEmpujados(),
            killRpt = PorcentajePisados(),
            music = MusicaSonando(),
            sound = EfectosSonando()
        };

        Debug.Log("[Analytics] GameOver | level=" + level
             + " | music=" + Booleano(MusicaSonando())
             + " | sound=" + Booleano(EfectosSonando())
             + " | time=" + TiempoDelNivel()
             + " | phaseAvoid=" + PhaseAvoid()
             + " | cordX=" + ColumnaDeMuerte(xJugador)
             + " | cordY=" + FilaDeMuerte(yJugador)
             + " | killer=" + killer
             + " | pushRpt=" + PorcentajeEmpujados()
             + " | killRpt=" + PorcentajePisados());

        Enviar(gameOver);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void SeguirLasEscenas()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
        SceneManager.sceneLoaded += AlCargarEscena;

        ReiniciarContadores();
        LogLevelStart(NivelActual());
    }

    private static void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (modo != LoadSceneMode.Single) return;

        ReiniciarContadores();

        LogLevelStart(NivelDeEscena(escena.name));
    }
}
using System;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using AnalyticsEvent = Unity.Services.Analytics.Event;

public class Eventos : MonoBehaviour
{
    public static Eventos Singleton;

    // Cola para guardar eventos que ocurran mientras IniciarAnalytics termina de conectar
    private static readonly Queue<AnalyticsEvent> colaEventos = new Queue<AnalyticsEvent>();
    private static bool recoleccionIniciada = false;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Llama a este método desde IniciarAnalytics.cs cuando termine de inicializar y otorgar consentimiento
    public static void NotificarServiciosListos()
    {
        recoleccionIniciada = true;

        while (colaEventos.Count > 0)
        {
            try
            {
                AnalyticsService.Instance.RecordEvent(colaEventos.Dequeue());
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Analytics] No se pudo enviar un evento encolado: {e.Message}");
            }
        }
    }

    #region Clases de Eventos Personalizados

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

    public class RateEvent : AnalyticsEvent
    {
        public RateEvent() : base("Rate") { }

        public int lore { set { SetParameter("lore", value); } }
        public int art { set { SetParameter("art", value); } }
        public int fun { set { SetParameter("fun", value); } }
    }

    #endregion

    #region Constantes

    public const string KillerVacio = "vacio";
    public const string KillerPatch = "patch";
    public const string KillerRepeater1 = "rpt1";
    public const string KillerRepeater2 = "rpt2";
    public const string KillerSentinel1 = "sen1";
    public const string KillerSentinel2 = "sen2";

    public const int SinNivel = 0;

    #endregion

    #region Variables y Contadores Estáticos

    private static int repeatersDelNivel;
    private static int repeatersEmpujados;
    private static int repeatersPisados;
    private static int dashesDelNivel;
    private static int evitadosConPhase;

    private static string ultimoKiller = "enemigo";

    #endregion

    #region Métodos de Utilidad

    public static int NivelActual()
    {
        return NivelDeEscena(SceneManager.GetActiveScene().name);
    }

    public static int NivelDeEscena(string nombreEscena)
    {
        const string prefijo = "Nivel";

        if (string.IsNullOrEmpty(nombreEscena) || !nombreEscena.StartsWith(prefijo)) return SinNivel;

        if (!int.TryParse(nombreEscena.Substring(prefijo.Length), out int numero)) return SinNivel;
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

    #endregion

    #region Control de Métricas / Métodos Públicos

    public static string killerEnemigo() => ultimoKiller;

    public static void SetKiller(string killer) => ultimoKiller = killer;

    public static void ContarRepeater() => repeatersDelNivel++;

    public static void ContarRepeaterEmpujado()
    {
        repeatersEmpujados++;
        Debug.Log($"[Analytics] (aviso) Repeater al vacio | {repeatersEmpujados} de {repeatersDelNivel} | pushRpt={PorcentajeEmpujados()}");
    }

    public static int PorcentajeEmpujados()
    {
        if (repeatersDelNivel <= 0) return 0;
        return Mathf.RoundToInt(100f * repeatersEmpujados / repeatersDelNivel);
    }

    public static void ContarRepeaterPisado()
    {
        repeatersPisados++;
        Debug.Log($"[Analytics] (aviso) Repeater pisado | {repeatersPisados} de {repeatersDelNivel} | killRpt={PorcentajePisados()}");
    }

    public static int PorcentajePisados()
    {
        if (repeatersDelNivel <= 0) return 0;
        return Mathf.RoundToInt(100f * repeatersPisados / repeatersDelNivel);
    }

    public static void ContarDash() => dashesDelNivel++;

    public static void ContarPhaseEvito()
    {
        evitadosConPhase++;
        Debug.Log($"[Analytics] (aviso) Phase evito dano | phaseAvoid={evitadosConPhase}");
    }

    public static int DashesDelNivel() => dashesDelNivel;

    public static int PhaseAvoid() => evitadosConPhase;

    private static void ReiniciarContadores()
    {
        repeatersDelNivel = 0;
        repeatersEmpujados = 0;
        repeatersPisados = 0;
        dashesDelNivel = 0;
        evitadosConPhase = 0;
    }

    #endregion

    #region Envío Seguro de Eventos

    private static void Enviar(AnalyticsEvent evento)
    {
        // Si IniciarAnalytics aún no ha terminado, encolamos el evento
        if (UnityServices.State != ServicesInitializationState.Initialized || !recoleccionIniciada)
        {
            colaEventos.Enqueue(evento);
            Debug.Log("[Analytics] Evento encolado mientras Unity Services termina de inicializar.");
            return;
        }

        try
        {
            AnalyticsService.Instance.RecordEvent(evento);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Analytics] No se pudo enviar el evento: {e.Message}");
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

        Debug.Log($"[Analytics] LevelStart | level={level} | music={Booleano(MusicaSonando())} | sound={Booleano(EfectosSonando())}");

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

        Debug.Log($"[Analytics] LevelComplete | level={level} | music={Booleano(MusicaSonando())} | sound={Booleano(EfectosSonando())} | time={TiempoDelNivel()} | phaseAvoid={PhaseAvoid()} | dash={DashesDelNivel()} | pushRpt={PorcentajeEmpujados()} | killRpt={PorcentajePisados()}");

        Enviar(levelComplete);
    }

    public static void LogGameOver(int level, float xJugador, string killer = "enemigo")
    {
        if (level == SinNivel) return;

        SetKiller(killer);

        Debug.Log($"[Analytics] GameOver | level={level} | music={Booleano(MusicaSonando())} | time={TiempoDelNivel()} | killer={ultimoKiller} | phaseAvoid={PhaseAvoid()} | cordX={ColumnaDeMuerte(xJugador)} | pushRpt={PorcentajeEmpujados()} | killRpt={PorcentajePisados()}");
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

        Debug.Log($"[Analytics] GameOver | level={level} | music={Booleano(MusicaSonando())} | sound={Booleano(EfectosSonando())} | time={TiempoDelNivel()} | phaseAvoid={PhaseAvoid()} | cordX={ColumnaDeMuerte(xJugador)} | cordY={FilaDeMuerte(yJugador)} | killer={killer} | pushRpt={PorcentajeEmpujados()} | killRpt={PorcentajePisados()}");

        Enviar(gameOver);
    }

    // Evento "Rate" de la hoja de diseño: se llama al apretar el botón de enviar de la ventana de calificar.
    // lore = historia, art = arte, fun = jugabilidad (la hoja los define de 1 a 5; 0 = sin calificar).
    // Se registra siempre que se aprieta enviar, aunque haya categorías en 0 (decisión del equipo).
    public static void LogRate(int lore, int art, int fun)
    {
        RateEvent rate = new RateEvent
        {
            lore = lore,
            art = art,
            fun = fun
        };

        bool completa = lore > 0 && art > 0 && fun > 0;

        Debug.Log($"[Analytics] Rate | lore={lore} | art={art} | fun={fun}" + (completa ? "" : " (hay categorias sin calificar)"));

        Enviar(rate);
    }

    #endregion

    #region Suscripción a Escenas

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    private static void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (modo != LoadSceneMode.Single) return;

        ReiniciarContadores();
        LogLevelStart(NivelDeEscena(escena.name));
    }

    #endregion
}
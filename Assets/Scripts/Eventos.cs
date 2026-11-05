using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;


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
        CustomEvent nombre = new CustomEvent("noombre del evento de Unity")
        {
            { "nombre del parametro",numeroNivelActual }
        }
        ;
    }

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

    private static int repeatersDelNivel;
    private static int repeatersEmpujados;

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

    private static void ReiniciarContadores()
    {
        repeatersDelNivel = 0;
        repeatersEmpujados = 0;
    }

    // Cuando se pase del log al envio real, la idea que dejo Dylan en el #987 es una
    // clase por evento con un campo por parametro. Las funciones de arriba son las que
    // van a llenar esos campos:
    //
    //   LevelCompleteEvent levelComplete = new LevelCompleteEvent
    //   {
    //       level = NivelActual(),
    //       time = TiempoDelNivel(),
    //       phaseAvoid = PhaseAvoid(),
    //       dash = DashesDelNivel(),
    //       pushRpt = PorcentajeEmpujados(),
    //       killRpt = PorcentajePisados(),
    //       music = MusicaSonando(),
    //       sound = EfectosSonando()
    //   };
    //
    //   AnalyticsService.Instance.RecordEvent(levelComplete);
    //
    // Antes hay que dar de alta el evento y sus parametros en el dashboard de Unity,
    // que es el primer punto de la historia #645.
    public static void LogLevelStart(int level)
    {
        if (level == SinNivel) return;

        Debug.Log("[Analytics] LevelStart | level=" + level
            + " | music=" + Booleano(MusicaSonando()));
    }

    public static void LogLevelComplete(int level)
    {
        if (level == SinNivel) return;

        Debug.Log("[Analytics] LevelComplete | level=" + level
            + " | music=" + Booleano(MusicaSonando())
            + " | time=" + TiempoDelNivel()
            + " | pushRpt=" + PorcentajeEmpujados());
    }

    public static void LogGameOver(int level, float xJugador)
    {
        if (level == SinNivel) return;

        Debug.Log("[Analytics] GameOver | level=" + level
            + " | music=" + Booleano(MusicaSonando())
            + " | time=" + TiempoDelNivel()
            + " | cordX=" + ColumnaDeMuerte(xJugador)
            + " | pushRpt=" + PorcentajeEmpujados());
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

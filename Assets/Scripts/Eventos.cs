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
        CustomEvent nombre/* de la variable(ejemplo level_start) */= new CustomEvent("noombre del evento de Unity")
        {
            { "nombre del parametro",numeroNivelActual }//agregar numero o el parametro que corresponda
        }
        ;

        //AnalyticsService.Instance.RecordEvent(nombre);
       
        //print(nombre del evento parametro = "" + nomeroNivelActual);

    }
    //Enviar el evento a cada codigo que corresponda

    // Parametro "level" de la hoja de diseno de eventos: INT, dominio { 1, 2, ... , 10 }.
    // Va en los tres eventos de nivel: LevelStart, LevelComplete y GameOver.
    // Sale del nombre de la escena ("Nivel7" da 7). En una escena que no es un nivel
    // (menu, creditos, calificar) devuelve 0 y no se registra nada.
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

    // Parametro "music" de la hoja de diseno: BOOL, dominio { true, false }.
    // True si la musica se estaba escuchando cuando se disparo el evento.
    // Volumen.Musica ya devuelve 0 cuando el boton de musica esta en mute, asi que
    // con mirar ese valor se cubren los dos casos: el mute general y el volumen en cero.
    public static bool MusicaSonando()
    {
        return Volumen.Musica > 0f;
    }

    // La hoja escribe los bool en minuscula; el ToString() de C# los escribe con mayuscula.
    private static string Booleano(bool valor)
    {
        return valor ? "true" : "false";
    }

    // Parametro "pushRpt" de la hoja de diseno: porcentaje de los Repeater 1.0 del nivel
    // que el jugador tiro al vacio empujandolos. Va en LevelComplete y en GameOver.
    // Cada Repeater 1.0 se anota al arrancar el nivel y se marca si termina cayendo
    // en la ZonaMuerte. Los contadores se reinician al cargar cada escena.
    private static int repeatersDelNivel;
    private static int repeatersEmpujados;

    public static void ContarRepeater()
    {
        repeatersDelNivel++;
    }

    // Este log no es un evento: es para poder ver el empujon en el momento en que pasa,
    // en vez de esperar al final del nivel para saber si se conto bien.
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

    // Por ahora los tres son solo logs, para comprobar que se disparan en el momento correcto
    // y una sola vez por nivel. Todavia no se manda nada a Unity Analytics: mas adelante aca va
    // el RecordEvent con estos parametros y los que agreguen las demas tareas del #645.
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
            + " | pushRpt=" + PorcentajeEmpujados());
    }

    public static void LogGameOver(int level)
    {
        if (level == SinNivel) return;

        Debug.Log("[Analytics] GameOver | level=" + level
            + " | music=" + Booleano(MusicaSonando())
            + " | pushRpt=" + PorcentajeEmpujados());
    }

    // LevelStart se dispara solo al cargar cada escena de nivel, asi no hace falta agregar
    // un objeto a las diez escenas. La llamada suelta de abajo cubre el caso de arrancar ya
    // dentro de un nivel, que es lo que pasa al darle Play en el editor.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void SeguirLasEscenas()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
        SceneManager.sceneLoaded += AlCargarEscena;

        LogLevelStart(NivelActual());
    }

    private static void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (modo != LoadSceneMode.Single) return;

        // sceneLoaded corre despues de los Awake de la escena nueva y antes de los Start,
        // asi que los contadores quedan en cero justo antes de que los Repeater se anoten.
        ReiniciarContadores();

        LogLevelStart(NivelDeEscena(escena.name));
    }
}
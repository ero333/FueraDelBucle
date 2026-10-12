using UnityEngine;

// Se agrega a la plataforma (o a una zona con trigger) donde tiene que arrancar la presentación.
// Cuando el jugador llega, la cámara hace un paneo suave hasta 'foco' (por ejemplo un Repeater),
// se queda ahí unos segundos y vuelve al jugador. Apenas vuelve aparece un marco con el video del
// enemigo (MarcoDesplegable) y el jugador se puede mover de nuevo cuando ese marco termina de cerrarse.
// Mientras tanto el jugador queda quieto en Idle (el juego no se pausa).
// El movimiento de la cámara lo hace CamaraSeguir.Paneo; PlayerPhysics frena al jugador con
// CamaraSeguir.PaneoActivo y MarcoDesplegable.Activo.
public class PaneoPresentacion : MonoBehaviour
{
    [Header("A qué mirar")]
    [Tooltip("Lo que muestra el paneo (por ejemplo el Repeater de la plataforma siguiente). Si se deja vacío se usa el Repeater más cercano hacia la derecha de este objeto.")]
    public Transform foco;

    [Header("Cámara")]
    [Tooltip("Si se deja vacío se busca la CamaraSeguir de la escena.")]
    public CamaraSeguir camara;

    [Header("Tiempos (segundos)")]
    [Tooltip("Cuánto tarda la cámara en ir hasta el foco. Arranca y frena de a poco.")]
    public float duracionIda = 1.8f;
    [Tooltip("Cuánto se queda la cámara quieta mirando el foco.")]
    public float segundosEnFoco = 2f;
    [Tooltip("Cuánto tarda la cámara en volver al jugador. Arranca y frena de a poco.")]
    public float duracionVuelta = 1.8f;

    [Header("Activación")]
    public string tagJugador = "Player";
    [Tooltip("Si está activado, la presentación se hace una sola vez por nivel.")]
    public bool soloUnaVez = true;

    [Header("Marco con video (al volver la cámara)")]
    [Tooltip("Apenas la cámara vuelve al jugador aparece un marco con un video; el jugador no se mueve hasta que el marco se cierra.")]
    public bool mostrarMarco = true;
    [Tooltip("Imagen del marco. Si se deja vacía se usa 'MarcoRepeater' (carpeta Resources/Presentacion).")]
    public Sprite imagenMarco;
    [Tooltip("Datos del video del marco (ruta dentro de Resources, sin extensión).")]
    public string datosVideoMarco = "Presentacion/Repeater1_video";
    [Tooltip("Dónde queda el centro del marco: 0 = izquierda / abajo, 1 = derecha / arriba (0,5 y 0,5 = justo en el medio).")]
    public Vector2 centroMarco = new Vector2(0.5f, 0.5f);
    [Tooltip("Alto del marco como fracción del alto de la pantalla.")]
    [Range(0.1f, 1f)]
    public float altoDelMarco = 0.54f;

    private const string ImagenMarcoPorDefecto = "Presentacion/MarcoRepeater";

    private bool yaMostrado;
    private MarcoDesplegable marcoDesplegable;

    private void Start()
    {
        // Se carga de entrada (el nivel recién arranca) para que no haya tirones cuando aparezca.
        if (mostrarMarco)
        {
            PrepararMarco();
        }
    }

    // Plataforma con collider sólido: cuenta cuando el jugador cae sobre ella (no si la toca de costado o desde abajo).
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(tagJugador)) return;

        Collider2D propio = collision.otherCollider;

        if (propio != null && collision.collider.bounds.center.y < propio.bounds.max.y) return;

        Iniciar();
    }

    // Zona con trigger: cuenta apenas el jugador entra.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(tagJugador)) return;

        Iniciar();
    }

    private void Iniciar()
    {
        if (soloUnaVez && yaMostrado) return;
        if (CamaraSeguir.PaneoActivo) return;

        if (camara == null)
        {
            camara = FindFirstObjectByType<CamaraSeguir>();
        }

        Transform objetivoPaneo = foco != null ? foco : BuscarRepeaterCercano();

        if (camara == null || objetivoPaneo == null)
        {
            Debug.LogWarning("PaneoPresentacion: falta la cámara (CamaraSeguir) o el foco del paneo en " + gameObject.name, this);
            return;
        }

        yaMostrado = true;

        if (mostrarMarco && marcoDesplegable == null)
        {
            PrepararMarco();
        }

        // Apenas la cámara termina de volver al jugador aparece el marco, sin un instante en el medio en que el jugador se pueda mover.
        System.Action alTerminar = null;

        if (marcoDesplegable != null)
        {
            alTerminar = marcoDesplegable.Mostrar;
        }

        camara.Paneo(objetivoPaneo, duracionIda, segundosEnFoco, duracionVuelta, alTerminar);
    }

    // Crea el marco (por código, aparte de esta plataforma) y carga sus imágenes.
    // Si falta algo no pasa nada: el paneo se hace igual, sin marco.
    private void PrepararMarco()
    {
        if (marcoDesplegable != null) return;

        Sprite imagen = imagenMarco != null ? imagenMarco : Resources.Load<Sprite>(ImagenMarcoPorDefecto);

        GameObject objeto = new GameObject("MarcoPresentacion");
        MarcoDesplegable nuevo = objeto.AddComponent<MarcoDesplegable>();

        nuevo.marco = imagen;
        nuevo.posicion = centroMarco;
        nuevo.alto = altoDelMarco;

        if (!nuevo.Cargar(datosVideoMarco))
        {
            Destroy(objeto);
            return;
        }

        marcoDesplegable = nuevo;
    }

    // El Repeater (normal o 2.0) más cercano que esté a la derecha de este objeto.
    private Transform BuscarRepeaterCercano()
    {
        Transform mejor = null;
        float mejorDistancia = float.MaxValue;

        foreach (DisparoEnemigo enemigo in FindObjectsByType<DisparoEnemigo>(FindObjectsSortMode.None))
        {
            Evaluar(enemigo.transform, ref mejor, ref mejorDistancia);
        }

        foreach (RepeaterSegundo enemigo in FindObjectsByType<RepeaterSegundo>(FindObjectsSortMode.None))
        {
            Evaluar(enemigo.transform, ref mejor, ref mejorDistancia);
        }

        return mejor;
    }

    private void Evaluar(Transform candidato, ref Transform mejor, ref float mejorDistancia)
    {
        float distancia = candidato.position.x - transform.position.x;

        if (distancia <= 0f || distancia >= mejorDistancia) return;

        mejor = candidato;
        mejorDistancia = distancia;
    }

    // En la ventana Scene, al seleccionar este objeto se ve una línea hasta el foco.
    private void OnDrawGizmosSelected()
    {
        if (foco == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, foco.position);
        Gizmos.DrawWireSphere(foco.position, 0.5f);
    }
}

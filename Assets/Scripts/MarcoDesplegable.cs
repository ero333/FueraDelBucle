using System;
using UnityEngine;
using UnityEngine.UI;

// Marco de interfaz con un video adentro. Aparece en dos pasos: primero asoma solo su borde izquierdo, que sube
// suave desde abajo de la pantalla, y después se despliega (hacia la derecha y hacia arriba). Cuando el marco va
// por la mitad del despliegue arranca el video y apenas termina el marco se pliega y baja.
// El video está partido en cuadros sueltos dentro de unas imágenes (carpeta Resources/Presentacion): así anda
// igual en la versión web y conserva la transparencia.
// Arma su propio Canvas por código (no hay que preparar nada en la escena) y queda por debajo del resto de la
// interfaz. No pausa el juego: mientras el marco está en pantalla MarcoDesplegable.Activo es verdadero y
// PlayerPhysics lo lee para dejar quieto al jugador.
public class MarcoDesplegable : MonoBehaviour
{
    // Descripción del video: la genera la conversión junto con las imágenes de los cuadros.
    [Serializable]
    public class DatosVideo
    {
        public int cuadros;
        public float fps;
        public int celdaAncho;
        public int celdaAlto;
        public int columnas;
        public int filasPorAtlas;
        public int cantidadAtlas;
        public string prefijoAtlas;

        // Zona que ocupa el video dentro de la imagen del marco (fracciones, origen arriba a la izquierda).
        public float areaX;
        public float areaY;
        public float areaAncho;
        public float areaAlto;
    }

    [Header("Imagen")]
    [Tooltip("Imagen del marco completo (con su borde).")]
    public Sprite marco;

    [Header("Posición en pantalla")]
    [Tooltip("Dónde queda el centro del marco: 0 = izquierda / abajo, 1 = derecha / arriba.")]
    public Vector2 posicion = new Vector2(0.5f, 0.5f);
    [Tooltip("Alto del marco como fracción del alto de la pantalla (el ancho sale de las proporciones de la imagen).")]
    [Range(0.1f, 1f)]
    public float alto = 0.54f;

    [Header("Borde que asoma primero")]
    [Tooltip("Ancho de la tira del borde, como fracción del ancho del marco.")]
    [Range(0.01f, 0.5f)]
    public float anchoBorde = 0.05f;
    [Tooltip("Alto de la tira del borde, como fracción del alto del marco (arranca desde abajo).")]
    [Range(0.1f, 1f)]
    public float altoBorde = 0.8f;

    [Header("Tiempos (segundos)")]
    [Tooltip("Cuánto tarda el borde en subir desde abajo hasta su lugar.")]
    public float duracionSubida = 0.7f;
    [Tooltip("Pausa entre que el borde llega y empieza a desplegarse.")]
    public float pausaEntreEtapas = 0.1f;
    [Tooltip("Cuánto tarda en desplegarse hasta mostrar el marco completo.")]
    public float duracionDespliegue = 0.8f;
    [Tooltip("En qué punto del despliegue arranca el video: 0 = apenas empieza a desplegarse, 1 = cuando termina. Con 0,625 el marco va por el 70 % de su ancho.")]
    [Range(0f, 1f)]
    public float inicioVideoEnDespliegue = 0.625f;
    [Tooltip("Cuánto tarda en plegarse cuando termina el video.")]
    public float duracionPlegado = 0.5f;
    [Tooltip("Pausa entre que se pliega y empieza a bajar.")]
    public float pausaAlCerrar = 0.05f;
    [Tooltip("Cuánto tarda en bajar y salir de la pantalla.")]
    public float duracionBajada = 0.5f;

    // Verdadero desde que se pide mostrar el marco hasta que termina de cerrarse.
    // PlayerPhysics lo lee para dejar al jugador quieto mientras tanto.
    public static bool Activo { get; private set; }

    // El Canvas siempre mide 1080 de alto, sin importar el tamaño de la ventana.
    private const float AnchoReferencia = 1920f;
    private const float AlturaReferencia = 1080f;

    // Va por debajo del resto de la interfaz (vidas, pausa, placas, diálogos), que usa orden 0 o más.
    private const int OrdenCanvas = -50;

    // Cuánto más abajo del borde de la pantalla arranca la tira del borde, para que no se vea antes de subir.
    private const float MargenOculto = 10f;

    private DatosVideo datos;
    private Texture2D[] atlas;

    private GameObject lienzo;
    private RectTransform tarjeta;
    private RectTransform mascara;
    private RawImage video;

    private float anchoMarco;
    private float altoMarco;
    private float desfaseOculto;

    private bool listo;
    private bool corriendo;
    private float tiempo;
    private int cuadroActual = -1;
    private int atlasActual = -1;

    // Segundo (desde que se pide mostrar) en que arranca el video.
    private float InicioVideo
    {
        get { return duracionSubida + pausaEntreEtapas + inicioVideoEnDespliegue * duracionDespliegue; }
    }

    // Segundo en que empieza a cerrarse: apenas termina el video (y nunca antes de haber terminado de abrirse).
    private float InicioCierre
    {
        get
        {
            float finDespliegue = duracionSubida + pausaEntreEtapas + duracionDespliegue;
            return Mathf.Max(InicioVideo + datos.cuadros / datos.fps, finDespliegue);
        }
    }

    private float DuracionTotal
    {
        get { return InicioCierre + duracionPlegado + pausaAlCerrar + duracionBajada; }
    }

    // Carga los datos y los cuadros del video (ver Resources/Presentacion) y arma el marco, todavía oculto.
    // Necesita que 'marco' ya esté asignado. Devuelve falso si falta algo: en ese caso no se muestra nada y el juego sigue normal.
    public bool Cargar(string rutaDatosVideo)
    {
        if (listo)
            return true;

        if (marco == null)
        {
            Debug.LogWarning("MarcoDesplegable: falta la imagen del marco.", this);
            return false;
        }

        TextAsset texto = Resources.Load<TextAsset>(rutaDatosVideo);

        if (texto == null)
        {
            Debug.LogWarning("MarcoDesplegable: no se encontró '" + rutaDatosVideo + "' en Resources.", this);
            return false;
        }

        datos = JsonUtility.FromJson<DatosVideo>(texto.text);

        if (datos == null || datos.cuadros <= 0 || datos.fps <= 0f || datos.columnas <= 0 || datos.filasPorAtlas <= 0
            || datos.cantidadAtlas <= 0 || datos.celdaAncho <= 1 || datos.celdaAlto <= 1)
        {
            Debug.LogWarning("MarcoDesplegable: los datos del video '" + rutaDatosVideo + "' no son válidos.", this);
            return false;
        }

        atlas = new Texture2D[datos.cantidadAtlas];

        for (int i = 0; i < atlas.Length; i++)
        {
            atlas[i] = Resources.Load<Texture2D>(datos.prefijoAtlas + i);

            if (atlas[i] == null)
            {
                Debug.LogWarning("MarcoDesplegable: falta la imagen '" + datos.prefijoAtlas + i + "' en Resources.", this);
                atlas = null;
                return false;
            }
        }

        Construir();
        listo = true;

        return true;
    }

    // Empieza la secuencia: sube el borde, se despliega, corre el video y se cierra.
    public void Mostrar()
    {
        if (!listo || corriendo)
            return;

        tiempo = 0f;
        corriendo = true;
        Activo = true;

        lienzo.SetActive(true);
        Aplicar();
    }

    // Corta la secuencia ya mismo y saca el marco de la pantalla.
    public void OcultarYa()
    {
        Terminar();
    }

    private void Update()
    {
        if (!corriendo)
            return;

        // Margen de 0.15s para evitar que la tecla 'E' que abre el diálogo cierre el video en el mismo frame.
        if (tiempo > 0.15f)
        {
            bool presionoTecla = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.X);

            // Verificación compatible con el nuevo Input System si está activo en el proyecto
#if ENABLE_INPUT_SYSTEM
            if (!presionoTecla && UnityEngine.InputSystem.Keyboard.current != null)
            {
                presionoTecla = UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame ||
                                UnityEngine.InputSystem.Keyboard.current.xKey.wasPressedThisFrame;
            }
#endif

            if (presionoTecla)
            {
                OcultarYa();
                return;
            }
        }

        tiempo += Time.deltaTime;

        if (tiempo >= DuracionTotal)
        {
            Terminar();
            return;
        }

        Aplicar();
    }

    private void Terminar()
    {
        corriendo = false;
        Activo = false;

        if (lienzo != null)
            lienzo.SetActive(false);
    }

    private void OnDisable()
    {
        // Si se apaga en medio de la secuencia, no se queda trabado el jugador.
        if (corriendo)
            Terminar();
    }

    private void OnDestroy()
    {
        if (corriendo)
        {
            corriendo = false;
            Activo = false;
        }
    }

    // Arma el Canvas: Canvas > Tarjeta (se mueve) > Máscara (recorta) > Imagen del marco + Video.
    // La máscara siempre tiene su esquina de abajo a la izquierda fija en la tarjeta, así que cuando crece
    // el marco y el video se quedan quietos y lo que se ve va creciendo desde el borde.
    private void Construir()
    {
        altoMarco = alto * AlturaReferencia;
        anchoMarco = altoMarco * (marco.rect.width / marco.rect.height);

        lienzo = new GameObject("MarcoCanvas", typeof(Canvas), typeof(CanvasScaler));
        lienzo.transform.SetParent(transform, false);

        Canvas canvas = lienzo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = OrdenCanvas;

        CanvasScaler escala = lienzo.GetComponent<CanvasScaler>();
        escala.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escala.referenceResolution = new Vector2(AnchoReferencia, AlturaReferencia);
        escala.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        escala.matchWidthOrHeight = 1f;

        // Tarjeta: ocupa el lugar del marco en la pantalla; sube y baja moviendo su posición.
        GameObject objetoTarjeta = new GameObject("Tarjeta", typeof(RectTransform));
        tarjeta = (RectTransform)objetoTarjeta.transform;
        tarjeta.SetParent(lienzo.transform, false);
        tarjeta.anchorMin = posicion;
        tarjeta.anchorMax = posicion;
        tarjeta.pivot = new Vector2(0.5f, 0.5f);
        tarjeta.sizeDelta = new Vector2(anchoMarco, altoMarco);

        // Máscara: recorta lo que se ve.
        GameObject objetoMascara = new GameObject("Mascara", typeof(RectTransform), typeof(RectMask2D));
        mascara = (RectTransform)objetoMascara.transform;
        mascara.SetParent(tarjeta, false);
        AnclarAbajoIzquierda(mascara);

        // Imagen del marco.
        GameObject objetoImagen = new GameObject("Imagen", typeof(RectTransform), typeof(Image));
        RectTransform rectImagen = (RectTransform)objetoImagen.transform;
        rectImagen.SetParent(mascara, false);
        AnclarAbajoIzquierda(rectImagen);
        rectImagen.sizeDelta = new Vector2(anchoMarco, altoMarco);

        Image imagen = objetoImagen.GetComponent<Image>();
        imagen.sprite = marco;
        imagen.raycastTarget = false;

        // Video: arriba de la imagen, en la zona que le corresponde dentro del marco.
        GameObject objetoVideo = new GameObject("Video", typeof(RectTransform), typeof(RawImage));
        RectTransform rectVideo = (RectTransform)objetoVideo.transform;
        rectVideo.SetParent(mascara, false);
        AnclarAbajoIzquierda(rectVideo);
        rectVideo.anchoredPosition = new Vector2(datos.areaX * anchoMarco, (1f - datos.areaY - datos.areaAlto) * altoMarco);
        rectVideo.sizeDelta = new Vector2(datos.areaAncho * anchoMarco, datos.areaAlto * altoMarco);

        video = objetoVideo.GetComponent<RawImage>();
        video.raycastTarget = false;

        // Fuera de la pantalla: la tira del borde (la parte de abajo del marco) tiene que quedar toda por debajo del borde de abajo.
        desfaseOculto = -MargenOculto + altoMarco * 0.5f - altoMarco * altoBorde - posicion.y * AlturaReferencia;

        // Nace apagado: Mostrar() lo enciende.
        lienzo.SetActive(false);
    }

    private static void AnclarAbajoIzquierda(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
    }

    // Deja el marco y el video como corresponde al instante actual de la secuencia.
    private void Aplicar()
    {
        float subida;
        float despliegue;

        CalcularEtapas(tiempo, out subida, out despliegue);

        tarjeta.anchoredPosition = new Vector2(0f, Mathf.Lerp(desfaseOculto, 0f, subida));

        mascara.sizeDelta = new Vector2(
            Mathf.Lerp(anchoMarco * anchoBorde, anchoMarco, despliegue),
            Mathf.Lerp(altoMarco * altoBorde, altoMarco, despliegue));

        // El primer cuadro del video ya está en el marco desde que empieza a abrirse; arranca a mitad del despliegue.
        float segundosDeVideo = tiempo - InicioVideo;
        int cuadro = segundosDeVideo <= 0f ? 0 : Mathf.Min(datos.cuadros - 1, (int)(segundosDeVideo * datos.fps));

        MostrarCuadro(cuadro);
    }

    // subida: 0 = la tira del borde fuera de la pantalla, 1 = en su lugar.
    // despliegue: 0 = solo la tira del borde, 1 = el marco completo.
    private void CalcularEtapas(float t, out float subida, out float despliegue)
    {
        float inicioDespliegue = duracionSubida + pausaEntreEtapas;
        float inicioCierre = InicioCierre;

        if (t < inicioCierre)
        {
            // Apertura: primero sube el borde y después se despliega (cada etapa arranca y frena de a poco).
            subida = Suavizado(duracionSubida > 0f ? t / duracionSubida : 1f);
            despliegue = Suavizado(duracionDespliegue > 0f ? (t - inicioDespliegue) / duracionDespliegue : 1f);
            return;
        }

        // Cierre: el camino inverso. Primero se pliega y después baja.
        float u = t - inicioCierre;

        if (u < duracionPlegado)
        {
            subida = 1f;
            despliegue = 1f - Suavizado(duracionPlegado > 0f ? u / duracionPlegado : 1f);
            return;
        }

        u -= duracionPlegado + pausaAlCerrar;

        despliegue = 0f;
        subida = 1f - Suavizado(duracionBajada > 0f ? u / duracionBajada : 1f);
    }

    // Arranca y termina despacio (0 a 1).
    private static float Suavizado(float k)
    {
        k = Mathf.Clamp01(k);

        return k * k * (3f - 2f * k);
    }

    // Muestra el cuadro 'cuadro' del video: lo busca en su imagen (atlas) y le dice al RawImage qué parte de la imagen mostrar.
    private void MostrarCuadro(int cuadro)
    {
        if (cuadro == cuadroActual)
            return;

        cuadroActual = cuadro;

        int porAtlas = datos.columnas * datos.filasPorAtlas;
        int cual = Mathf.Min(cuadro / porAtlas, atlas.Length - 1);
        int local = cuadro - cual * porAtlas;
        int columna = local % datos.columnas;
        int fila = local / datos.columnas;

        Texture2D textura = atlas[cual];

        if (cual != atlasActual)
        {
            video.texture = textura;
            atlasActual = cual;
        }

        // Los cuadros se acomodaron de arriba hacia abajo y las coordenadas de la textura empiezan abajo.
        // Media unidad de píxel de margen en cada lado para que el filtro no mezcle cuadros vecinos.
        float anchoTextura = textura.width;
        float altoTextura = textura.height;

        float u = (columna * datos.celdaAncho + 0.5f) / anchoTextura;
        float v = (altoTextura - (fila + 1) * datos.celdaAlto + 0.5f) / altoTextura;

        video.uvRect = new Rect(u, v, (datos.celdaAncho - 1f) / anchoTextura, (datos.celdaAlto - 1f) / altoTextura);
    }
}
using UnityEngine;

public class PatchFondo : MonoBehaviour
{
    [Header("Para Nivel 6 (Usa parallaxController)")]
    public parallaxController parallaxNivel6;
    [Tooltip("Índice de la capa del fondo (0 = primer hijo, 1 = segundo, etc.)")]
    public int layerIndexNivel6 = 0;

    [Header("Para Nivel 10 (Patch suelto en jerarquía)")]
    public bool usarEnNivel10 = false;
    [Range(0f, 1f)]
    [Tooltip("0 = fijo en el suelo | 0.85 = acompaña el fondo lejano | 1 = pegado a la cámara en X")]
    public float seguimientoHorizontal10 = 0.85f;
    [Tooltip("Suavizado al subir/bajar plataformas (valores bajos como 0.8 o 1.2 evitan que brinque cuando saltas)")]
    public float suavizadoVertical10 = 1.0f;

    private Transform cam;

    // Variables internas Nivel 6
    private Renderer layerRenderer;
    private float layerSpeed6;
    private Vector3 initialLocalOffset6;

    // Variables internas Nivel 10
    private Vector3 initialCameraPos10;
    private Vector3 initialPatchPos10;
    private float targetY10;

    void Start()
    {
        cam = Camera.main.transform;

        if (parallaxNivel6 != null && layerIndexNivel6 < parallaxNivel6.transform.childCount)
        {
            initialLocalOffset6 = transform.position - cam.position;
            Transform targetLayer = parallaxNivel6.transform.GetChild(layerIndexNivel6);
            layerRenderer = targetLayer.GetComponent<Renderer>();

            float farthestBack = 0f;
            int count = parallaxNivel6.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                float zDiff = parallaxNivel6.transform.GetChild(i).position.z - cam.position.z;
                if (zDiff > farthestBack) farthestBack = zDiff;
            }

            float myZDiff = targetLayer.position.z - cam.position.z;
            float backSpeed = 1f - (myZDiff / farthestBack);
            layerSpeed6 = backSpeed * parallaxNivel6.parallaxSpeed;
        }

        // Lógica Nivel 10
        if (usarEnNivel10)
        {
            initialCameraPos10 = cam.position;
            initialPatchPos10 = transform.position;
            targetY10 = transform.position.y;
        }
    }

    void LateUpdate()
    {
        // Ejecución Nivel 6
        if (parallaxNivel6 != null && layerRenderer != null)
        {
            float distance = cam.position.x - parallaxNivel6.referenceX;
            float uvOffset = distance * layerSpeed6;
            float worldDisplacementX = uvOffset * layerRenderer.bounds.size.x;

            float finalX = cam.position.x + initialLocalOffset6.x - worldDisplacementX;
            float finalY = cam.position.y + initialLocalOffset6.y;

            transform.position = new Vector3(finalX, finalY, transform.position.z);
        }
        // Ejecución Nivel 10
        else if (usarEnNivel10)
        {
            float deltaCamX = cam.position.x - initialCameraPos10.x;
            float deltaCamY = cam.position.y - initialCameraPos10.y;

            float finalX = initialPatchPos10.x + (deltaCamX * seguimientoHorizontal10);
            float desiredY = initialPatchPos10.y + (deltaCamY * seguimientoHorizontal10);

            targetY10 = Mathf.Lerp(targetY10, desiredY, Time.deltaTime * suavizadoVertical10);

            transform.position = new Vector3(finalX, targetY10, transform.position.z);
        }
    }
}
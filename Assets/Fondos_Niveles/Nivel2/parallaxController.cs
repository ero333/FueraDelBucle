using UnityEngine;

public class parallaxController : MonoBehaviour
{
    Transform cam;
    float distance;

    GameObject[] backgrounds;
    Material[] mat;
    float[] backSpeed;

    float farthestBack;

    [Range(0.01f, 0.05f)]
    public float parallaxSpeed;

    // X de la cámara donde el fondo se ve "sin desplazar" (inicio del nivel)
    public float referenceX = 0f;

    // Cuánto a la derecha de la cámara se coloca el centro del fondo
    public float offsetX = 34f;

    float startZ;

    void Start()
    {
        cam = Camera.main.transform;
        startZ = transform.position.z;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backgrounds = new GameObject[backCount];

        for (int i = 0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = backgrounds[i].GetComponent<Renderer>().material;
        }
        BackSpeedCalculate(backCount);
    }

    void BackSpeedCalculate(int backCount)
    {
        for (int i = 0; i < backCount; i++)
        {
            if ((backgrounds[i].transform.position.z - cam.position.z) > farthestBack)
            {
                farthestBack = backgrounds[i].transform.position.z - cam.position.z;
            }
        }

        for (int i = 0; i < backCount; i++)
        {
            backSpeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    private void LateUpdate()
    {
        distance = cam.position.x - referenceX;
        transform.position = new Vector3(cam.position.x + offsetX, transform.position.y, startZ);

        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;
            mat[i].SetTextureOffset("_MainTex", new Vector2(distance, 0) * speed);
        }
    }
}
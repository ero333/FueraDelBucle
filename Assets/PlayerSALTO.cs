using UnityEngine;

public class PlayerSALTO : MonoBehaviour
{
    public float fuerzasalto;
    public float velocidad;
    public bool enSuelo;
    public float longitudRaycast = 0.1f;

    public LayerMask Suelo;

    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, Suelo);
        enSuelo = hit.collider != null;

        if (enSuelo && Input.GetKeyDown(KeyCode.Space)) 
        {
            rb.AddForce(new Vector2(0f, fuerzasalto), ForceMode2D.Impulse);

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }
}

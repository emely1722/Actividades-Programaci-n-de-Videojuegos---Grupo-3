using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    public float velocidad = 2f;
    private bool moviendoADerecha = true;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
     
        float direccion = moviendoADerecha ? 1f : -1f;
        rb.linearVelocity = new Vector2(direccion * velocidad, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.name.Contains("Limite"))
        {
            moviendoADerecha = !moviendoADerecha;

         
            Vector3 escala = transform.localScale;
            escala.x *= -1;
            transform.localScale = escala;
        }
    }
}

using UnityEngine;

public class ComplexEnemy : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 2f;
    private bool moviendoADerecha = true;
    private Rigidbody2D rb;
    private Animator animator; 

    [Header("Configuración de Persecución y Ataque")]
    public Transform player;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;  

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 

        if (player == null)
        {
            GameObject jugadorGO = GameObject.FindGameObjectWithTag("Player");
            if (jugadorGO != null) player = jugadorGO.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (animator != null) animator.SetBool("isWalking", false);

            if (animator != null) animator.SetTrigger("attack");
        }

     
        else if (distanceToPlayer <= detectionRange)
        {
 
            Vector3 direction = (player.position - transform.position).normalized;

            direction.y = 0;

            if (direction.x > 0 && !moviendoADerecha)
                Flip();
            else if (direction.x < 0 && moviendoADerecha)
                Flip();

            if (animator != null) animator.SetBool("isWalking", true);

            transform.Translate(direction * velocidad * Time.deltaTime);
        }

        else
        {
       
            if (animator != null) animator.SetBool("isWalking", true);

            float direccion = moviendoADerecha ? 1f : -1f;
            rb.linearVelocity = new Vector2(direccion * velocidad, rb.linearVelocity.y);
        }
    }

    void Flip()
    {
        moviendoADerecha = !moviendoADerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > detectionRange)
        {
            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.name.Contains("Limite"))
            {
                Flip();
            }
        }
    }
}
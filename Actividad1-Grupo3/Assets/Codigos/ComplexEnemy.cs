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

    // VARIABLE NUEVA: Evita que el trigger de ataque se ejecute repetidamente en cada frame
    private bool yaAtaco = false;

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

        // ==========================================
        // ESTADO 1: ATACAR (Muy cerca del jugador)
        // ==========================================
        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Frena los pies

            if (animator != null)
            {
                animator.SetBool("isWalking", false); // Apaga caminata

                // CORRECCIÓN: Solo disparamos el trigger si no se ha disparado ya
                if (!yaAtaco)
                {
                    animator.SetTrigger("attack"); // ¡Dispara el espadazo!
                    yaAtaco = true;                // Cerramos el candado
                }
            }
        }
        // ==========================================
        // ESTADO 2: PERSEGUIR (Cerca del jugador)
        // ==========================================
        else if (distanceToPlayer <= detectionRange)
        {
            yaAtaco = false; // Al alejarse, se desbloquea el candado para poder volver a atacar

            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // Evita que flote

            if (direction.x > 0 && !moviendoADerecha) Flip();
            else if (direction.x < 0 && moviendoADerecha) Flip();

            if (animator != null) animator.SetBool("isWalking", true);

            transform.Translate(direction * velocidad * Time.deltaTime);
        }
        // ==========================================
        // ESTADO 3: PATRULLAR (Jugador lejos)
        // ==========================================
        else
        {
            yaAtaco = false; // Desbloquea el candado por si acaso

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
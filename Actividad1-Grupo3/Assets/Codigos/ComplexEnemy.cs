using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ComplexEnemy : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;

    [Header("Jugador")]
    [SerializeField] private Transform player;

    [Header("Rangos")]
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float attackRange = 2f;

    [Header("Ataque")]
    [SerializeField] private float tiempoEntreAtaques = 1.2f;
    [SerializeField] private int dano = 20;

    [Header("Debug")]
    [SerializeField] private bool mostrarDebug = false;

    private Rigidbody2D rb;
    private Animator animator;

    private bool moviendoADerecha = true;
    private float siguienteAtaque;

    private readonly int isWalkingHash =
        Animator.StringToHash("isWalking");

    private readonly int attackStateHash =
        Animator.StringToHash("Attack");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Busca el Animator incluso si está en un hijo.
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError(
                "ComplexEnemy: no se encontró un Animator.",
                gameObject
            );
        }
    }

    private void Start()
    {
        // Busca automáticamente al jugador.
        if (player == null)
        {
            GameObject jugadorGO =
                GameObject.FindGameObjectWithTag("Player");

            if (jugadorGO != null)
            {
                player = jugadorGO.transform;
            }
            else
            {
                Debug.LogError(
                    "ComplexEnemy: no se encontró un objeto con Tag Player."
                );
            }
        }

        // Comprueba que exista el estado Attack.
        if (animator != null &&
            !animator.HasState(0, attackStateHash))
        {
            Debug.LogError(
                "ComplexEnemy: el Animator no tiene un estado llamado exactamente 'Attack'.",
                gameObject
            );
        }
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        float distanciaJugador =
            Vector2.Distance(
                rb.position,
                player.position
            );

        // ============================
        // ATAQUE
        // ============================

        if (distanciaJugador <= attackRange)
        {
            Atacar();
            return;
        }

        // ============================
        // PERSECUCIÓN
        // ============================

        if (distanciaJugador <= detectionRange)
        {
            Perseguir();
            return;
        }

        // ============================
        // PATRULLA
        // ============================

        Patrullar();
    }

    private void Atacar()
    {
        // Detenerse.
        rb.linearVelocity =
            new Vector2(0f, rb.linearVelocity.y);

        MirarJugador();

        if (animator != null)
        {
            animator.SetBool(isWalkingHash, false);
        }

        if (mostrarDebug)
        {
            Debug.Log("Enemigo dentro del rango de ataque.");
        }

        // Todavía está esperando para atacar otra vez.
        if (Time.time < siguienteAtaque)
            return;

        siguienteAtaque =
            Time.time + tiempoEntreAtaques;

        if (animator != null)
        {
            if (animator.HasState(0, attackStateHash))
            {
                if (mostrarDebug)
                {
                    Debug.Log(
                        "REPRODUCIENDO ANIMACIÓN ATTACK"
                    );
                }

                // Reproduce directamente el espadazo.
                animator.Play(
                    attackStateHash,
                    0,
                    0f
                );
            }
            else
            {
                Debug.LogError(
                    "No existe el estado 'Attack' en la capa 0 del Animator."
                );
            }
        }
    }

    private void Perseguir()
    {
        if (animator != null)
        {
            animator.SetBool(isWalkingHash, true);
        }

        float direccion;

        if (player.position.x >
            transform.position.x)
        {
            direccion = 1f;
        }
        else
        {
            direccion = -1f;
        }

        // Mirar al jugador.
        if (direccion > 0f &&
            !moviendoADerecha)
        {
            Flip();
        }
        else if (
            direccion < 0f &&
            moviendoADerecha
        )
        {
            Flip();
        }

        rb.linearVelocity =
            new Vector2(
                direccion * velocidad,
                rb.linearVelocity.y
            );
    }

    private void Patrullar()
    {
        if (animator != null)
        {
            animator.SetBool(isWalkingHash, true);
        }

        float direccion =
            moviendoADerecha ? 1f : -1f;

        rb.linearVelocity =
            new Vector2(
                direccion * velocidad,
                rb.linearVelocity.y
            );
    }

    private void MirarJugador()
    {
        if (player == null)
            return;

        if (player.position.x >
            transform.position.x)
        {
            if (!moviendoADerecha)
            {
                Flip();
            }
        }
        else
        {
            if (moviendoADerecha)
            {
                Flip();
            }
        }
    }

    private void Flip()
    {
        moviendoADerecha =
            !moviendoADerecha;

        Vector3 escala =
            transform.localScale;

        escala.x *= -1f;

        transform.localScale =
            escala;
    }

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        if (player == null)
            return;

        float distanciaJugador =
            Vector2.Distance(
                transform.position,
                player.position
            );

        if (distanciaJugador <= detectionRange)
        {
            return;
        }

        if (
            collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.name.Contains("Limite")
        )
        {
            Flip();
        }
    }

    public void GolpearJugador()
    {
        if (player == null)
            return;

        float distancia =
            Vector2.Distance(
                transform.position,
                player.position
            );

        // Un pequeño margen adicional.
        if (distancia > attackRange + 0.4f)
            return;

        PlayerHealth vidaJugador =
            player.GetComponentInParent<PlayerHealth>();

        if (vidaJugador != null)
        {
            vidaJugador.RecibirDanio(dano);

            if (mostrarDebug)
            {
                Debug.Log(
                    "ESPADAZO: el jugador recibió " +
                    dano +
                    " de daño."
                );
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Amarillo = detección
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );

        // Rojo = ataque
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}
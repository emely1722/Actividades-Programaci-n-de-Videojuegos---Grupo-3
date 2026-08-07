using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDroneMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float distance = 3f;

    [Header("Detección y disparo")]
    [SerializeField] private float rangoAtaque = 8f;
    [SerializeField] private float tiempoEntreDisparos = 2f;

   
    [SerializeField] private Proyectil proyectilPrefab;

    [SerializeField] private Transform puntoDisparo;

    [Header("Orientación")]
    [Tooltip("Actívalo si el sprite original del dron mira hacia la derecha.")]
    [SerializeField] private bool spriteOriginalMiraDerecha = true;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform jugador;

    private float startingX;
    private bool movingRight = true;
    private float tiempoSiguienteDisparo;

    private float puntoDisparoXInicial;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        startingX = rb.position.x;

        if (puntoDisparo != null)
        {
            puntoDisparoXInicial =
                Mathf.Abs(puntoDisparo.localPosition.x);
        }
    }

    private void Start()
    {
        GameObject jugadorGO =
            GameObject.FindGameObjectWithTag("Player");

        if (jugadorGO != null)
        {
            jugador = jugadorGO.transform;
        }
        else
        {
            Debug.LogError(
                "EnemyDroneMovement: no se encontró un objeto con Tag Player."
            );
        }

        ActualizarPuntoDisparo();
    }

    private void FixedUpdate()
    {
        if (jugador != null)
        {
            float distanciaJugador = Vector2.Distance(
                rb.position,
                jugador.position
            );

            // Cuando el jugador entra en rango, deja de patrullar,
            // apunta y dispara.
            if (distanciaJugador <= rangoAtaque)
            {
                ApuntarAJugador();

                if (Time.time >= tiempoSiguienteDisparo)
                {
                    Disparar();

                    tiempoSiguienteDisparo =
                        Time.time + tiempoEntreDisparos;
                }

                return;
            }
        }

        Patrullar();
    }

    private void Patrullar()
    {
        float targetX = movingRight
            ? startingX + distance
            : startingX - distance;

        Vector2 targetPosition =
            new Vector2(targetX, rb.position.y);

        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            targetPosition,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);

        if (Vector2.Distance(
                rb.position,
                targetPosition
            ) <= 0.02f)
        {
            ChangeDirection();
        }
    }

    private void ApuntarAJugador()
    {
        if (jugador.position.x > transform.position.x &&
            !movingRight)
        {
            ChangeDirection();
        }
        else if (
            jugador.position.x < transform.position.x &&
            movingRight)
        {
            ChangeDirection();
        }
    }

    private void Disparar()
    {
        if (animator != null)
        {
            animator.SetTrigger("attack");
        }

        if (proyectilPrefab == null)
        {
            Debug.LogError(
                "EnemyDroneMovement: falta asignar el prefab de la bolita.",
                this
            );

            return;
        }

        if (jugador == null)
            return;

        Vector2 origen = puntoDisparo != null
            ? puntoDisparo.position
            : transform.position;

        Vector2 direccion =
            ((Vector2)jugador.position - origen).normalized;

        Proyectil nuevoProyectil = Instantiate(
            proyectilPrefab,
            origen,
            Quaternion.identity
        );

        nuevoProyectil.EstablecerDireccion(direccion);

        // Impide que la bolita choque inmediatamente
        // con el propio dron.
        Collider2D colliderDrone =
            GetComponent<Collider2D>();

        Collider2D colliderProyectil =
            nuevoProyectil.GetComponent<Collider2D>();

        if (colliderDrone != null &&
            colliderProyectil != null)
        {
            Physics2D.IgnoreCollision(
                colliderDrone,
                colliderProyectil
            );
        }
    }

    private void ChangeDirection()
    {
        movingRight = !movingRight;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX =
                !spriteRenderer.flipX;
        }

        ActualizarPuntoDisparo();
    }

    private void ActualizarPuntoDisparo()
    {
        if (puntoDisparo == null ||
            spriteRenderer == null)
        {
            return;
        }

        bool mirandoDerecha;

        if (spriteOriginalMiraDerecha)
        {
            mirandoDerecha =
                !spriteRenderer.flipX;
        }
        else
        {
            mirandoDerecha =
                spriteRenderer.flipX;
        }

        Vector3 posicionLocal =
            puntoDisparo.localPosition;

        posicionLocal.x = mirandoDerecha
            ? puntoDisparoXInicial
            : -puntoDisparoXInicial;

        puntoDisparo.localPosition =
            posicionLocal;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            rangoAtaque
        );
    }
}
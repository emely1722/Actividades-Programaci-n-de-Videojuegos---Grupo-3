using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDroneMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float distance = 3f;

    [Header("Detección y Disparo")]
    [SerializeField] private float rangoAtaque = 8f;
    [SerializeField] private float tiempoEntreDisparos = 2f;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform puntoDisparo;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform jugador;

    private float startingX;
    private bool movingRight = true;
    private float tiempoSiguienteDisparo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        startingX = rb.position.x;
    }

    private void Start()
    {
        GameObject jugadorGO = GameObject.FindGameObjectWithTag("Player");
        if (jugadorGO != null)
        {
            jugador = jugadorGO.transform;
        }
    }

    private void FixedUpdate()
    {
        if (jugador != null)
        {
            float distanciaJugador = Vector2.Distance(rb.position, jugador.position);

            // dron dispara
            if (distanciaJugador <= rangoAtaque)
            {
                ApuntarAJugador();

                if (Time.time >= tiempoSiguienteDisparo)
                {
                    Disparar();
                    tiempoSiguienteDisparo = Time.time + tiempoEntreDisparos;
                }
                return;
            }
        }

        // patrulla de un lado a otro
        Patrullar();
    }

    private void Patrullar()
    {
        float targetX = movingRight ? startingX + distance : startingX - distance;
        Vector2 targetPosition = new Vector2(targetX, rb.position.y);

        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            targetPosition,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);

        if (Vector2.Distance(rb.position, targetPosition) <= 0.02f)
        {
            ChangeDirection();
        }
    }

    private void ApuntarAJugador()
    {
        if (jugador.position.x > transform.position.x && !movingRight)
        {
            ChangeDirection();
        }
        else if (jugador.position.x < transform.position.x && movingRight)
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

        if (laserPrefab == null)
        {
            Debug.LogError("error");
            return;
        }

        if (jugador == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) jugador = p.transform;
        }

        Vector3 origen = puntoDisparo != null ? puntoDisparo.position : transform.position;

        origen.z = 0f;

        GameObject laser = Instantiate(laserPrefab, origen, Quaternion.identity);

        Vector2 direccion = Vector2.left; 
        if (jugador != null)
        {
            direccion = (jugador.position - origen).normalized;
        }

        LaserProyectil proyectil = laser.GetComponent<LaserProyectil>();
        if (proyectil != null)
        {
            proyectil.EstablecerDireccion(direccion);
        }
        else
        {
            Debug.LogError("error");
        }
    }

    private void ChangeDirection()
    {
        movingRight = !movingRight;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}
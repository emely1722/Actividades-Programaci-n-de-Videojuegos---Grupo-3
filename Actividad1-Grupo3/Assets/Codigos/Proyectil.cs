using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Proyectil : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float velocidad = 7f;
    [SerializeField] private float tiempoDeVida = 3f;
    [SerializeField] private int dano = 20;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    // Este es el método que EnemyDroneMovement necesita.
    public void EstablecerDireccion(Vector2 nuevaDireccion)
    {
        Vector2 direccionNormalizada = nuevaDireccion.normalized;

        rb.linearVelocity =
            direccionNormalizada * velocidad;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Evita que la bolita golpee al propio dron.
        if (collision.GetComponentInParent<EnemyDroneMovement>() != null)
        {
            return;
        }

        PlayerHealth vidaJugador =
            collision.GetComponentInParent<PlayerHealth>();

        if (vidaJugador != null)
        {
            vidaJugador.RecibirDanio(dano);
            Destroy(gameObject);
            return;
        }

        // Se destruye al tocar pisos, paredes o plataformas sólidas.
        if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DroneMovimiento : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform jugador;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2.5f;
    [SerializeField] private float distanciaDeteccion = 12f;
    [SerializeField] private float distanciaMinima = 5f;
    [SerializeField] private bool seguirVerticalmente = true;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (jugador == null)
        {
            GameObject objetoJugador =
                GameObject.FindGameObjectWithTag("Player");

            if (objetoJugador != null)
            {
                jugador = objetoJugador.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        if (jugador == null)
            return;

        Vector2 direccion =
            (Vector2)jugador.position - rb.position;

        float distancia = direccion.magnitude;

        // No moverse si el jugador está lejos.
        if (distancia > distanciaDeteccion)
            return;

        // Detenerse a cierta distancia para poder disparar.
        if (distancia <= distanciaMinima)
            return;

        direccion.Normalize();

        // Desactiva esto si quieres que solo se mueva horizontalmente.
        if (!seguirVerticalmente)
        {
            direccion.y = 0f;
        }

        Vector2 nuevaPosicion =
            rb.position +
            direccion * velocidad * Time.fixedDeltaTime;

        rb.MovePosition(nuevaPosicion);
    }
}
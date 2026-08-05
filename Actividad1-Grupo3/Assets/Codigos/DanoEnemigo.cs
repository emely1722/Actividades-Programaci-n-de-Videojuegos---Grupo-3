using UnityEngine;

public class DanoEnemigo : MonoBehaviour
{
    [SerializeField] private int dano = 20;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth jugador =
            collision.collider.GetComponentInParent<PlayerHealth>();

        if (jugador != null)
        {
            jugador.RecibirDanio(dano);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth jugador =
            other.GetComponentInParent<PlayerHealth>();

        if (jugador != null)
        {
            jugador.RecibirDanio(dano);
        }
    }
}
using UnityEngine;

public class DanoEnemigo : MonoBehaviour
{
    [SerializeField] private int dano = 20;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth vidaJugador =
            collision.collider.GetComponentInParent<PlayerHealth>();

        if (vidaJugador != null)
        {
            vidaJugador.RecibirDano(dano);
        }
    }
}
using UnityEngine;

public class LaserProyectil : MonoBehaviour
{
    [SerializeField] private float velocidad = 8f;
    [SerializeField] private int danio = 15;
    [SerializeField] private float tiempoVida = 4f;

    private Vector2 direccionDisparo;

    private void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    private void Update()
    {
        transform.Translate(direccionDisparo * velocidad * Time.deltaTime, Space.World);
    }

    public void EstablecerDireccion(Vector2 direccion)
    {
        direccionDisparo = direccion.normalized;

        float angulo = Mathf.Atan2(direccionDisparo.y, direccionDisparo.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("impacto: " + other.gameObject.name);

        if (other.CompareTag("Enemy") || other.name.Contains("Drone") || other.name.Contains("Laser"))
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador");

            other.SendMessageUpwards("RecibirDanio", danio, SendMessageOptions.DontRequireReceiver);

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Ground") || other.name.Contains("Plataforma") || other.name.Contains("Limite"))
        {
            Destroy(gameObject);
        }
    }
}
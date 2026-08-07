using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    [SerializeField] private int vidaActual;

    [Header("Interfaz")]
    [SerializeField] private Slider barraVida;

    [Header("Protección después de recibir daño")]
    [SerializeField] private float tiempoInvulnerable = 1f;

    [Header("Animación de daño")]
    [SerializeField] private Animator animator;
    [SerializeField] private string triggerDanio = "Hurt";

    private bool esInvulnerable;
    private bool estaMuerto;

    private Coroutine rutinaInvulnerabilidad;

    private void Awake()
    {
        vidaActual = vidaMaxima;

        // Busca automáticamente el Animator si no fue asignado.
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        ActualizarBarra();
    }

    // Quita vida al jugador.
    public void RecibirDanio(int cantidad)
    {
        if (cantidad <= 0 || esInvulnerable || estaMuerto)
        {
            return;
        }

        // Se activa inmediatamente para impedir golpes duplicados.
        esInvulnerable = true;

        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(
            vidaActual,
            0,
            vidaMaxima
        );

        ActualizarBarra();
        ReproducirAnimacionDanio();

        Debug.Log(
            "Daño recibido: " + cantidad +
            " | Vida actual: " + vidaActual
        );

        if (vidaActual <= 0)
        {
            Morir();
            return;
        }

        if (rutinaInvulnerabilidad != null)
        {
            StopCoroutine(rutinaInvulnerabilidad);
        }

        rutinaInvulnerabilidad =
            StartCoroutine(ActivarInvulnerabilidad());
    }

    // Recupera vida sin superar la cantidad máxima.
    public void RecuperarVida(int cantidad)
    {
        if (cantidad <= 0 || estaMuerto)
        {
            return;
        }

        vidaActual += cantidad;
        vidaActual = Mathf.Clamp(
            vidaActual,
            0,
            vidaMaxima
        );

        ActualizarBarra();

        Debug.Log(
            "Vida recuperada: " + cantidad +
            " | Vida actual: " + vidaActual
        );
    }

    private void ActualizarBarra()
    {
        if (barraVida == null)
        {
            return;
        }

        barraVida.minValue = 0;
        barraVida.maxValue = vidaMaxima;
        barraVida.value = vidaActual;
    }

    private void ReproducirAnimacionDanio()
    {
        if (animator == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(triggerDanio))
        {
            return;
        }

        animator.ResetTrigger(triggerDanio);
        animator.SetTrigger(triggerDanio);
    }

    private IEnumerator ActivarInvulnerabilidad()
    {
        yield return new WaitForSeconds(
            tiempoInvulnerable
        );

        esInvulnerable = false;
        rutinaInvulnerabilidad = null;
    }

    private void Morir()
    {
        if (estaMuerto)
        {
            return;
        }

        estaMuerto = true;
        StopAllCoroutines();

        Debug.Log("El jugador murió.");

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
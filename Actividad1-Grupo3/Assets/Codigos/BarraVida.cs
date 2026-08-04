using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    [SerializeField] private int vidaActual;

    [Header("Interfaz")]
    [SerializeField] private Slider barraVida;

    [Header("Protección después de recibir daño")]
    [SerializeField] private float tiempoInvulnerable = 1f;

    private bool esInvulnerable;

    private void Start()
    {
        vidaActual = vidaMaxima;
        ActualizarBarra();
    }

    // quita vida al jugador
    public void RecibirDanio(int cantidad)
    {
        if (esInvulnerable || vidaActual <= 0)
        {
            return;
        }

        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        ActualizarBarra();

        if (vidaActual <= 0)
        {
            Morir();
        }
        else
        {
            StartCoroutine(ActivarInvulnerabilidad());
        }
    }

    // le devuelve vida sin superar la vida máxima
    public void RecuperarVida(int cantidad)
    {
        if (vidaActual <= 0)
        {
            return;
        }

        vidaActual += cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        ActualizarBarra();
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

    private IEnumerator ActivarInvulnerabilidad()
    {
        esInvulnerable = true;

        yield return new WaitForSeconds(tiempoInvulnerable);

        esInvulnerable = false;
    }

    private void Morir()
    {
        // escena solamente se reinicia cuando la vida llega a cero
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
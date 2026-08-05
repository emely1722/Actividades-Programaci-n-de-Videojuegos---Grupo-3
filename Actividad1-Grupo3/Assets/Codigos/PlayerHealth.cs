using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    [SerializeField] private int vidaActual;

    [Header("Barra de vida")]
    [SerializeField] private Slider barraVida;

    [Header("Invulnerabilidad")]
    [SerializeField] private float tiempoInvulnerable = 1f;

    private bool invulnerable;
    private bool muerto;

    private void Awake()
    {
        vidaActual = vidaMaxima;

        if (barraVida != null)
        {
            barraVida.minValue = 0;
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vidaActual;
        }
    }

    public void RecibirDanio(int cantidad)
    {
        if (invulnerable || muerto || cantidad <= 0)
            return;

        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        ActualizarBarra();

        Debug.Log("Vida actual: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
        else
        {
            StartCoroutine(InvulnerabilidadTemporal());
        }
    }

    public void RecuperarVida(int cantidad)
    {
        if (muerto || cantidad <= 0)
            return;

        vidaActual += cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        ActualizarBarra();

        Debug.Log("Vida recuperada. Vida actual: " + vidaActual);
    }

    private void ActualizarBarra()
    {
        if (barraVida != null)
        {
            barraVida.value = vidaActual;
        }
    }

    private IEnumerator InvulnerabilidadTemporal()
    {
        invulnerable = true;

        yield return new WaitForSeconds(tiempoInvulnerable);

        invulnerable = false;
    }

    private void Morir()
    {
        muerto = true;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
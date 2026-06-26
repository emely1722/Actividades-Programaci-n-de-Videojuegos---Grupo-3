using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float jumpForce = 7f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float maxFallSpeed = -15f;

    [Header("Jump Juice")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("UI")]
    public TMP_Text contadorJuego;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private int apple = 0;
    private int banana = 0;

    private List<GameObject> objetosRecolectados = new List<GameObject>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        ActualizarUI();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Velocidad horizontal", Mathf.Abs(moveInput));

        if (moveInput > 0) spriteRenderer.flipX = false;
        else if (moveInput < 0) spriteRenderer.flipX = true;

        if (isGrounded) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space)) jumpBufferCounter = jumpBufferTime;
        else jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("IsJumping", true);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (rb.linearVelocity.y < maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("IsJumping", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (objetosRecolectados.Contains(other.gameObject))
        {
            return;
        }

        // Control de Manzanas
        if (other.CompareTag("CollectibleApple"))
        {
            objetosRecolectados.Add(other.gameObject); 
            apple++;
            ActualizarUI();
            StartCoroutine(Recolectar(other.gameObject));
        }
        // Control de Bananas
        else if (other.CompareTag("CollectibleBanana"))
        {
            objetosRecolectados.Add(other.gameObject);
            banana++;
            ActualizarUI();
            StartCoroutine(Recolectar(other.gameObject));
        }
    }

    private IEnumerator Recolectar(GameObject objeto)
    {
        Collider2D colisionador = objeto.GetComponent<Collider2D>();
        if (colisionador != null)
        {
            colisionador.enabled = false;
        }

        Animator anim = objeto.GetComponent<Animator>();
        if (anim != null)
        {
            foreach (AnimatorControllerParameter param in anim.parameters)
            {
                if (param.name == "Collect")
                {
                    anim.SetTrigger("Collect");
                    break;
                }
            }
        }

        yield return new WaitForSeconds(0.35f);

        if (objetosRecolectados.Contains(objeto))
        {
            objetosRecolectados.Remove(objeto);
        }

        Destroy(objeto);
    }

    private void ActualizarUI()
    {
        if (contadorJuego != null)
        {
            contadorJuego.text = "Apple: " + apple + " | Banana: " + banana;
        }
    }
}
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDroneMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float distance = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private float startingX;
    private bool movingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        startingX = rb.position.x;
    }

    private void FixedUpdate()
    {
        float targetX;

        if (movingRight)
        {
            targetX = startingX + distance;
        }
        else
        {
            targetX = startingX - distance;
        }

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

    private void ChangeDirection()
    {
        movingRight = !movingRight;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }
}
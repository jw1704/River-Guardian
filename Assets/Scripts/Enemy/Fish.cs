using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public float maxMoveSpeed = 2f;
    public float accelerationRate = 0.5f;
    public float smoothTime = 0.3f;
    public float rotationSpeed = 5f;

    private GameObject targetLixo;
    private Vector2 velocity = Vector2.zero;
    private SpriteRenderer spriteRenderer;
    private float currentMoveSpeed = 0f; // Velocidade atual, começa em 0

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (targetLixo != null)
        {
            // Aumenta gradualmente a velocidade até o limite
            currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, maxMoveSpeed, accelerationRate * Time.deltaTime);
            Move(targetLixo.transform);
        }
        else
        {
            targetLixo = GameObject.FindGameObjectWithTag("Lixo");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lixo"))
        {
            Destroy(collision.gameObject); // Destroi o objeto com a tag "Lixo"
            targetLixo = null; // Libera a referência do alvo após destruí-lo
            currentMoveSpeed = 0f; // Reseta a velocidade ao colidir com o lixo
        }
    }

    public void Move(Transform targetTransform)
    {
        Vector2 direction = ((Vector2)targetTransform.position - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Move na direção do alvo com a velocidade atual
        transform.position = (Vector2)transform.position + direction * currentMoveSpeed * Time.deltaTime;

        Vector2 clampedPosition = new Vector2(
            Mathf.Clamp(transform.position.x, -10f, 10f),
            Mathf.Clamp(transform.position.y, -10f, 0.5f)
        );
        transform.position = clampedPosition;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipY = direction.x < 0;
        }
    }
}

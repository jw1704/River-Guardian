using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("HideInInspector")]
    private Rigidbody2D rb;
    private Animator anim;
    private Camera cam;

    [Header("Camera")]
    [SerializeField] private Vector3 offset;

    [Header("Status")]
    [SerializeField] public float currentMovementSpeed = 2.5f;

    [Header("VFX")]
    [SerializeField] private GameObject raycastVFXPrefab;
    [SerializeField] private Transform VFX_SpawnTransform;

    [Header("Trash Detection")]
    [SerializeField] private GameObject Check;

    [Header("Player Appearance")]
    [SerializeField] private Sprite currentPlayerSprite;
    private SpriteRenderer spriteRenderer;


    private void OnEnable()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (currentPlayerSprite != null)
            spriteRenderer.sprite = currentPlayerSprite;
    }

    public void UpdatePlayerSprite(Sprite newSprite)
    {
        currentPlayerSprite = newSprite;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = currentPlayerSprite;
        }
    }

    private void FixedUpdate()
    {
        MovementManager();
        Camera2DManager();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            FishingManager();
        }
        CheckTrash();
    }

    private void Camera2DManager()
    {
        Vector3 desiredPosition = transform.position + offset;

        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(desiredPosition.x, -3, 3),
            Mathf.Clamp(desiredPosition.y, -7, 10),
            desiredPosition.z
        );

        Vector3 smoothedPosition = Vector3.Lerp(cam.transform.position, clampedPosition, 0.03f);
        cam.transform.position = smoothedPosition;
    }

    private void MovementManager()
    {
        float moveInput = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(moveInput, 0, 0) * currentMovementSpeed * Time.deltaTime;

        transform.position += movement;

        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(transform.position.x, -10, 10),
            transform.position.y,
            transform.position.z
        );

        transform.position = clampedPosition;
    }

    private void FishingManager()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, ~LayerMask.GetMask("Player"));

        if (hit.collider != null && hit.collider.CompareTag("Lixo"))
        {
            Destroy(hit.collider.gameObject);

            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.OnTrashCollected();
            }

            if (raycastVFXPrefab != null && VFX_SpawnTransform != null)
            {
                Instantiate(raycastVFXPrefab, VFX_SpawnTransform.position, Quaternion.identity);
            }
        }
    }

    private void CheckTrash()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, ~LayerMask.GetMask("Player"));

        if (hit.collider != null && hit.collider.CompareTag("Lixo"))
        {
            Check.SetActive(true);
        }
        else
        {
            Check.SetActive(false);
        }
    }
}

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarriageHandler : MonoBehaviour
{
    [Header("TimeConfiguration")]
    [SerializeField] public float travelTime = 5f;

    [Header("World Positions")]
    [SerializeField] private Vector2 worldStartPosition;
    [SerializeField] private Vector2 worldEndPosition;

    private Rigidbody2D rb;
    private float timer;
    private bool isMoving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnEnable()
    {
        StartMovement();
    }

    public void StartMovement()
    {
        timer = 0f;
        isMoving = true;
        rb.position = worldStartPosition;
    }

    private void FixedUpdate()
    {
        if (!isMoving)
            return;

        timer += Time.fixedDeltaTime;

        float t = Mathf.Clamp01(timer / travelTime);
        Vector2 newPosition = Vector2.Lerp(
            worldStartPosition,
            worldEndPosition,
            t
        );

        rb.MovePosition(newPosition);

        if (t >= 1f)
        {
            isMoving = false;
            OnReachedDestination();
        }
    }

    private void OnReachedDestination()
    {
        // Hook para lógica futura
        // Ej: volver, desaparecer, evento de gameplay, etc.
    }
}

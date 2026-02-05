using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarriageHandler : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private UIManager uiManager; 

    [Header("World Positions")]
    [SerializeField] private Vector2 worldStartPosition;
    [SerializeField] private Vector2 worldEndPosition;

    private Rigidbody2D rb;
    private float timer;
    private bool isMoving;
    private float travelTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void StartMovement()
    {
        if (uiManager == null)
        {
            Debug.LogError("UIManager no está asignado en CarriageHandler!");
            return;
        }
        
        // Obtener el tiempo del UIManager
        travelTime = uiManager.GameTimeInSeconds;
        
        timer = 0f;
        isMoving = true;
        rb.position = worldStartPosition;
    }

    private void FixedUpdate()
    {
        if (!isMoving || uiManager == null)
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
        Debug.Log("Carroza llegó al destino (medianoche)"); 
    }
}

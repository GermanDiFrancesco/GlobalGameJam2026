using UnityEngine;

public class SuspectHandler : MonoBehaviour
{
    [Header("Dependencias")]
    [SerializeField] public GameController gameController;

    [Header("Mascara del asesino")]
    [SerializeField] public bool isKiller;

    [Header("Renderers de la Máscara")]
    [SerializeField] private SpriteRenderer nose;
    [SerializeField] private SpriteRenderer eyes;
    [SerializeField] private SpriteRenderer mouth;

    [Header("Movimiento aleatorio tipo 'wandering'")]
    [SerializeField] private float moveRadiusPixels = 50f;
    [SerializeField] private float pixelsPerUnit = 100f;
    [Tooltip("Velocidad máxima en unidades del mundo")]
    [SerializeField] private float maxSpeed = 1f;
    [Tooltip("Magnitud del jitter aleatorio (unidades/seg)")]
    [SerializeField] private float jitter = 1f;
    [Tooltip("Fuerza para mantener al NPC cerca del origen")]
    [SerializeField] private float returnStrength = 4f;
    [Tooltip("Freno aplicado a la velocidad cada frame (0..1, 1=no freno)")]
    [SerializeField] private float damping = 0.98f;
    [Tooltip("Rect de límites en unidades del mundo (x,y,width,height)")]
    [SerializeField] private Rect movementBounds = new Rect(-5f, -5f, 10f, 10f);

    private Vector2 origin;
    private Vector2 velocity;
    private float moveRadiusUnits;

    private void Start()
    {
        origin = transform.position;
        moveRadiusUnits = moveRadiusPixels / Mathf.Max(1f, pixelsPerUnit);
        // ajustar velocidad máxima por defecto relativa al radio si el usuario no lo cambia
        if (maxSpeed <= 0f) maxSpeed = moveRadiusUnits / 2f;
        velocity = Vector2.zero;
    }

    private void Update()
    {
        Vector2 pos = (Vector2)transform.position;

        // jitter aleatorio suave (wander)
        Vector2 randomJitter = Random.insideUnitCircle * jitter * Time.deltaTime;
        velocity += randomJitter;

        // empuje suave hacia el origen si se acerca a salirse del radio permitido
        Vector2 toOrigin = origin - pos;
        float dist = toOrigin.magnitude;
        if (dist > moveRadiusUnits)
        {
            // fuerza proporcional al exceso de distancia
            Vector2 pull = toOrigin.normalized * returnStrength * (dist - moveRadiusUnits) * Time.deltaTime;
            velocity += pull;
        }

        // limitar velocidad
        if (velocity.sqrMagnitude > maxSpeed * maxSpeed)
            velocity = velocity.normalized * maxSpeed;

        // aplicar damping para que parezca caminata más natural
        velocity *= damping;

        // mover
        pos += velocity * Time.deltaTime;

        // mantener dentro de los bounds del mundo
        pos = ClampToBounds(pos);

        transform.position = pos;
    }

    private Vector2 ClampToBounds(Vector2 pos)
    {
        float minX = movementBounds.xMin;
        float maxX = movementBounds.xMax;
        float minY = movementBounds.yMin;
        float maxY = movementBounds.yMax;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        return pos;
    }

    public void ApplyMaskFromData(string maskData)
    {
        int eyeIndex = int.Parse(maskData[0].ToString());
        int noseIndex = int.Parse(maskData[1].ToString());
        int mouthIndex = int.Parse(maskData[2].ToString());

        eyes.sprite = gameController.AntifaceSprites[eyeIndex];
        nose.sprite = gameController.HatSprites[noseIndex];
        mouth.sprite = gameController.AccesorySprites[mouthIndex];
    }
}

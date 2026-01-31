using UnityEngine;

public class SuspectHandler : MonoBehaviour
{   
    [Header("Identidad")]
    [SerializeField] public bool isKiller;
    [SerializeField] public bool isWitness;
    [SerializeField] private Clue assignedClue;

    [Header("Dependencies")]
    [SerializeField] private GameController gameController;

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer hatRenderer;
    [SerializeField] private SpriteRenderer ornamentRenderer;
    [SerializeField] private SpriteRenderer eyesRenderer;

    [Header("Movimiento aleatorio tipo 'wandering'")]
    private float moveRadiusPixels = 50f;
    private float pixelsPerUnit = 100f;
    private float maxSpeed = 1f;
    private float jitter = 1f;
    private float returnStrength = 4f;
    private float damping = 0.98f;
    private Rect movementBounds = new Rect(-5f, -5f, 200f, 10f);

    private Vector2 origin;
    private Vector2 velocity;
    private float moveRadiusUnits;

    #region Inicialización pública (GameController)

    /// <summary>
    /// Inicializa completamente al NPC desde el GameController
    /// </summary>
    public void Initialize(
        GameController controller,
        MaskIdentity identity,
        bool killer,
        Vector3 worldPosition
    )
    {
        gameController = controller;
        isKiller = killer;

        transform.position = worldPosition;
        origin = worldPosition;

        ApplyIdentity(identity);
    }
    public void SetAsWitness(Clue clue)
    {
        assignedClue = clue;
        isWitness = true;
    }
    public void DebugWitness()
    {
        if (!isWitness) return;

        Debug.Log(
            $"WITNESS → {assignedClue.part.type}:{assignedClue.part.index}"
        );
    }

    #endregion

    #region Identidad visual

    private void ApplyIdentity(MaskIdentity identity)
    {
        foreach (var part in identity.parts)
        {
            // reconstruimos un MaskPartId SOLO para pedir el sprite
            MaskPartId partId = new MaskPartId
            {
                type = part.Key,
                index = part.Value
            };

            Sprite sprite = gameController.GetSprite(partId);

            switch (part.Key)
            {
                case MaskPartType.Hat:
                    hatRenderer.sprite = sprite;
                    break;

                case MaskPartType.Ornament:
                    ornamentRenderer.sprite = sprite;
                    break;

                case MaskPartType.Eyes:
                    eyesRenderer.sprite = sprite;
                    break;
            }
        }
    }


    #endregion

    #region Movimiento

    private void Start()
    {
        moveRadiusUnits = moveRadiusPixels / Mathf.Max(1f, pixelsPerUnit);

        if (maxSpeed <= 0f)
            maxSpeed = moveRadiusUnits / 2f;

        velocity = Vector2.zero;
    }

    private void Update()
    {
        Vector2 pos = transform.position;

        Vector2 randomJitter = Random.insideUnitCircle * jitter * Time.deltaTime;
        velocity += randomJitter;

        Vector2 toOrigin = origin - pos;
        float dist = toOrigin.magnitude;

        if (dist > moveRadiusUnits)
        {
            Vector2 pull =
                toOrigin.normalized *
                returnStrength *
                (dist - moveRadiusUnits) *
                Time.deltaTime;

            velocity += pull;
        }

        if (velocity.sqrMagnitude > maxSpeed * maxSpeed)
            velocity = velocity.normalized * maxSpeed;

        velocity *= damping;
        pos += velocity * Time.deltaTime;
        pos = ClampToBounds(pos);

        transform.position = pos;
    }

    private Vector2 ClampToBounds(Vector2 pos)
    {
        pos.x = Mathf.Clamp(pos.x, movementBounds.xMin, movementBounds.xMax);
        pos.y = Mathf.Clamp(pos.y, movementBounds.yMin, movementBounds.yMax);
        return pos;
    }

    #endregion
}

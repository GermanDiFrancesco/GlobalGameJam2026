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

    [Header("Movimiento aleatorio")]
    [SerializeField] private float moveRadiusPixels = 50f;
    [SerializeField] private float pixelsPerUnit = 100f;
    [Tooltip("Intervalo en segundos para cambiar dirección")]
    [SerializeField] private float changeInterval = 2f;
    [Tooltip("Rect de límites en unidades del mundo (x,y,width,height)")]
    [SerializeField] private Rect movementBounds = new Rect(-5f, -5f, 10f, 10f);

    private Vector2 origin;
    private Vector2 target;
    private float moveRadiusUnits;
    private float moveSpeed;

    private void Start()
    {
        origin = transform.position;
        moveRadiusUnits = moveRadiusPixels / Mathf.Max(1f, pixelsPerUnit);
        moveSpeed = moveRadiusUnits / Mathf.Max(0.0001f, changeInterval); // intenta llegar en el intervalo
        PickNewTarget();
        InvokeRepeating(nameof(PickNewTarget), changeInterval, changeInterval);
    }

    private void Update()
    {
        Vector2 pos = transform.position;
        pos = Vector2.MoveTowards(pos, target, moveSpeed * Time.deltaTime);
        pos = ClampToBounds(pos);
        transform.position = pos;
    }

    private void PickNewTarget()
    {
        // elegir punto aleatorio alrededor del origen dentro del radio
        Vector2 randomOffset = Random.insideUnitCircle * moveRadiusUnits;
        Vector2 candidate = origin + randomOffset;
        target = ClampToBounds(candidate);
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

        eyes.sprite = gameController.eyeSprites[eyeIndex];
        nose.sprite = gameController.noseSprites[noseIndex];
        mouth.sprite = gameController.mouthSprites[mouthIndex];
    }
}

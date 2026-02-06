using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public interface IInteractable
{
    void OnInteracted();
    void SetInteractionIndicator(bool show);
    void SetInteractionProgress(float normalized); // 0 → 1
    void ClearInteractionProgress();
    Transform GetTransform();
    InteractionType GetInteractionType();
}
public enum InteractionType
{
    Accuse,
    Investigate,
    Cinematic
}

public class SuspectHandler : MonoBehaviour, IInteractable
{   
    [Header("Identity")]
    [SerializeField] public bool isKiller;
    [SerializeField] public bool isWitness;
    [SerializeField] private Clue assignedClue;

    [Header("Witness")]
    [SerializeField] public float delayWitnessing;
    [SerializeField] private bool isWitnessAlready = false;
    [SerializeField] private bool givedClue = false;

    [Header("Dependencies")]
    [SerializeField] private GameController gameController;

    [Header("Sprites")]
    [SerializeField] private Sprite body;
    [SerializeField] private Sprite bodyHandsUp;

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer hatRenderer;
    [SerializeField] private SpriteRenderer ornamentRenderer;
    [SerializeField] private SpriteRenderer eyesRenderer;
    //[SerializeField] private SpriteRenderer clueRenderer;
    [SerializeField] private SpriteRenderer bodyRenderer;

    [Header("Interaction Indicators")]
    [SerializeField] private GameObject speechRoot;
    [SerializeField] private GameObject indicatorRoot;
    [SerializeField] private Image interactionFill; // fill radial worldspace
    [SerializeField] private Image interaction;
    [SerializeField] private Image clueImg;

    [Header("Interaction Resources")]
    [SerializeField] private Sprite interrogateSprite;
    [SerializeField] private Sprite accuseSprite;
    [SerializeField] private Color interrogateColor;
    [SerializeField] private Color accuseColor;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.4f;
    [SerializeField] private float maxDistanceFromSpawn = 3f;
    [SerializeField] private float maxIdleTime = 6f;
    private const float MinIdleTime = 3f;
    private const float MaxMoveTime = 6f;

    [Header("Walk Animation")]
    [SerializeField] private float frameInterval = 0.2f;
    [SerializeField] private float movementThreshold = 0.05f;
    [SerializeField] private float directionThreshold = 0.01f;
    [SerializeField] private SpriteRenderer legsRenderer;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite walkSprite;
    private float timer;
    private bool mirroredStep;

    private Rigidbody2D rb;
    private Vector2 _spawnPosition;
    private Vector2 _moveDir;
    private enum NPCState { Idle, Moving }
    private NPCState _currentState;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        _spawnPosition = transform.position;
    }
    private void Start()
    {
        StartCoroutine(DelayInicial());
        StartCoroutine(StateMachine());
    }
    private void Update()
    {
        HandleWalkAnimation();
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = _moveDir * moveSpeed;
    }

    #region Initialization

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
        ApplyIdentity(identity);
    }

    public void SetAsWitness(Clue clue)
    {
        assignedClue = clue;
        isWitness = true;
        clueImg.sprite = clue.sprite;
        interactionFill.sprite = interrogateSprite;
        interactionFill.color = interrogateColor;
        interaction.sprite = interrogateSprite;

        delayWitnessing = Random.Range(3f, 20f);
        if (isWitness) StartCoroutine(DelayedWitnessing(delayWitnessing));
    }
    private IEnumerator DelayedWitnessing(float delay)
    {
        yield return new WaitForSeconds(delay);
        isWitnessAlready = true;
        bodyRenderer.sprite = bodyHandsUp;
        moveSpeed = 0;
        SetInteractionIndicator(true);
    }
    public void DebugWitness()
    {
        if (!isWitness) return;

        Debug.Log(
            $"WITNESS → {assignedClue.part.type}:{assignedClue.part.index}"
        );
    }
    public void SetInteractionIndicator(bool show)
    {
        if (!givedClue)
        {
            if (!indicatorRoot) return;
            indicatorRoot.SetActive(show);

            if (!show) return;
        }
        //if (isWitnessAlready) indicatorRoot.SetActive(false);
    }
    public void SetInteractionProgress(float normalized)
    {
        if (interactionFill == null) return;
        interactionFill.fillAmount = Mathf.Clamp01(normalized);
        interactionFill.enabled = true;
    }
    public void ClearInteractionProgress()
    {
        if (interactionFill == null) return;
        interactionFill.fillAmount = 0f;
        interactionFill.enabled = false;
    }

    public void OnInteracted()
    {
        if (isWitness)
        {
            gameController.OnPlayerInvestigate(assignedClue);
            givedClue = true;
            indicatorRoot.SetActive(false);
            speechRoot.SetActive(true);
            clueImg.enabled = true;
            return;
        }
        gameController.OnPlayerAccuse(this);
    }

    #endregion

    #region Visuals

    private void ApplyIdentity(MaskIdentity identity)
    {
        interactionFill.sprite = accuseSprite;
        interactionFill.color = accuseColor;
        interaction.sprite = accuseSprite;

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

    private void HandleWalkAnimation()
    {
        if (rb == null || legsRenderer == null)
            return;

        HandleDirectionFlip();

        if (rb.linearVelocity.magnitude > movementThreshold)
        {
            AnimateWalk();
        }
        else
        {
            SetIdle();
        }
    }

    private void AnimateWalk()
    {
        timer += Time.deltaTime;

        if (timer >= frameInterval)
        {
            timer = 0f;
            mirroredStep = !mirroredStep;

            legsRenderer.sprite = walkSprite;
            legsRenderer.flipX = mirroredStep ? !legsRenderer.flipX : legsRenderer.flipX;
        }
    }

    private void SetIdle()
    {
        timer = 0f;
        mirroredStep = false;
        legsRenderer.sprite = idleSprite;
    }

    private void HandleDirectionFlip()
    {
        float vx = rb.linearVelocity.x;

        if (Mathf.Abs(vx) < directionThreshold)
            return;

        // Dirección base (mirar izquierda/derecha)
        bool facingLeft = vx < 0f;

        // Importante: el flip de dirección es la base
        legsRenderer.flipX = facingLeft ^ mirroredStep;
    }

    #endregion

    #region Movement
    private IEnumerator StateMachine()
    {
        while (true)
        {
            switch (_currentState)
            {
                case NPCState.Idle:
                    yield return StartCoroutine(IdleState());
                    break;

                case NPCState.Moving:
                    yield return StartCoroutine(MovingState());
                    break;
            }
        }
    }
    
    private IEnumerator DelayInicial()
    {
        float idleTime = Random.Range(0f, 10f);
        yield return new WaitForSeconds(idleTime);

        _currentState = NPCState.Moving;
    }
    
    private IEnumerator IdleState()
    {
        _moveDir = Vector2.zero;

        // Tiempo de idle entre 3 y maxIdleTime
        float idleTime = Random.Range(MinIdleTime, maxIdleTime);
        yield return new WaitForSeconds(idleTime);

        _currentState = NPCState.Moving;
    }
    private IEnumerator MovingState()
    {
        ChooseNewDirection();

        // Tiempo de movimiento entre 0 y el máximo calculado
        float moveTime = Random.Range(0f, MaxMoveTime);
        yield return new WaitForSeconds(moveTime);

        _currentState = NPCState.Idle;
    }
    private void ChooseNewDirection()
    {
        const int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            // Generamos dirección aleatoria
            Vector2 randomDirection = new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;

            // Proyectamos usando el máximo tiempo calculado
            Vector2 futurePosition = (Vector2)transform.position +
                                    randomDirection * maxDistanceFromSpawn;

            // Verificamos si está dentro del radio
            if (Vector2.Distance(futurePosition, _spawnPosition) <= maxDistanceFromSpawn)
            {
                _moveDir = randomDirection;
                return;
            }
        }

        // Si no encontramos dirección válida, volvemos al spawn
        _moveDir = (_spawnPosition - (Vector2)transform.position).normalized;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 center = Application.isPlaying ? _spawnPosition : (Vector2)transform.position;

        // Círculo del radio máximo (amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, maxDistanceFromSpawn);

        if (Application.isPlaying)
        {
            // Estado actual: rojo (idle) o verde (moving)
            Gizmos.color = _currentState == NPCState.Idle ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }

    public Transform GetTransform() => transform;

    public InteractionType GetInteractionType()
    {
        return isWitness ? InteractionType.Investigate : InteractionType.Accuse;
    }

    #endregion

}

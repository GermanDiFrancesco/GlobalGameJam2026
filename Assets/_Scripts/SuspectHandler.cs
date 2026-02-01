using System.Collections;
using UnityEngine;

public class SuspectHandler : MonoBehaviour
{   
    [Header("Identity")]
    [SerializeField] public bool isKiller;
    [SerializeField] public bool isWitness;
    [SerializeField] private Clue assignedClue;

    [Header("Dependencies")]
    [SerializeField] private GameController gameController;

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer hatRenderer;
    [SerializeField] private SpriteRenderer ornamentRenderer;
    [SerializeField] private SpriteRenderer eyesRenderer;
    [SerializeField] private SpriteRenderer clueRenderer;

    [Header("Interaction Indicators")]
    [SerializeField] private GameObject speechRoot;
    [SerializeField] private GameObject indicatorRoot;
    [SerializeField] private GameObject accuseIndicator;
    [SerializeField] private GameObject investigateIndicator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.4f;
    [SerializeField] private float maxDistanceFromSpawn = 3f;
    [SerializeField] private float maxIdleTime = 6f;
    private const float MinIdleTime = 3f;
    private const float MaxMoveTime = 6f;

    private Rigidbody2D _rigidbody2D;
    private Vector2 _spawnPosition;
    private Vector2 _moveDir;
    private enum NPCState { Idle, Moving }
    private NPCState _currentState;

    public void OnInteracted()
    {
        if (isWitness)
        {
            gameController.OnPlayerInvestigate(assignedClue);
            return;
        }
        gameController.OnPlayerAccuse(this);
    }

    #region Initialization

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

        ApplyIdentity(identity);
    }
    public void SetAsWitness(Clue clue)
    {
        assignedClue = clue;
        isWitness = true;

        speechRoot.SetActive(true);
        clueRenderer.sprite = clue.sprite;
        clueRenderer.enabled = true;
    }
    public void DebugWitness()
    {
        if (!isWitness) return;

        Debug.Log(
            $"WITNESS → {assignedClue.part.type}:{assignedClue.part.index}"
        );
    }

    #endregion

    #region Visuals

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

    public void SetInteractionIndicator(bool show)
    {
        if (!indicatorRoot) return;
        indicatorRoot.SetActive(show);

        if (!show) return;

        //accuseIndicator.SetActive(!isWitness);
        //investigateIndicator.SetActive(isWitness);
    }


    #endregion

    #region Movement

    private void Awake()
    {
        _rigidbody2D = GetComponentInChildren<Rigidbody2D>();
        _spawnPosition = transform.position;
    }
    private void Start()
    {
        StartCoroutine(DelayInicial());
        StartCoroutine(StateMachine());
    }

    private void FixedUpdate()
    {
        _rigidbody2D.linearVelocity = _moveDir * moveSpeed;
    }

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

    #endregion

}

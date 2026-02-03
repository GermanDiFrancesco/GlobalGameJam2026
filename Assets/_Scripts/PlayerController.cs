using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float MoveSpeed = 20f;
    [SerializeField] private float InteractHoldTime = 1f;
    [SerializeField] private float InteractRadius = 1.25f;

    private Rigidbody2D _rigidbody2D;
    private Vector3 _moveDir;

    private float _interactTimer;
    private SuspectHandler _currentTarget;
    private SuspectHandler _lastHighlightedTarget;

    [Header("Inputs")]
    [SerializeField] private KeyCode upKey = KeyCode.W;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode downKey = KeyCode.S;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode interactKey = KeyCode.Space;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        InputHandle_Movement();
        InputHandle_Interaction();
    }

    private void FixedUpdate()
    {
        _rigidbody2D.linearVelocity = _moveDir * MoveSpeed;
    }

    // ================= INPUT HANDLING =================

    private void InputHandle_Movement()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(upKey)) moveY = +1f;
        if (Input.GetKey(downKey)) moveY = -1f;
        if (Input.GetKey(leftKey)) moveX = -1f;
        if (Input.GetKey(rightKey)) moveX = +1f;

        _moveDir = new Vector3(moveX, moveY).normalized;
    }

    private void InputHandle_Interaction()
    {
        UpdateInteractionTarget();

        if (Input.GetKey(interactKey))
        {
            if (_currentTarget == null)
            {
                _currentTarget = FindNearbySuspect();
                _interactTimer = 0f;
            }

            if (_currentTarget != null)
            {
                _interactTimer += Time.deltaTime;

                // TODO: enviar progreso a UI (_interactTimer / InteractHoldTime)

                if (_interactTimer >= InteractHoldTime)
                {
                    _currentTarget.OnInteracted();
                    ResetInteraction();
                }
            }
        }

        if (Input.GetKeyUp(interactKey))
        {
            ResetInteraction();
        }
    }

    private void ResetInteraction()
    {
        _interactTimer = 0f;
        _currentTarget = null;
    }

    private void UpdateInteractionTarget()
    {
        SuspectHandler nearby = FindNearbySuspect();

        if (nearby != _lastHighlightedTarget)
        {
            if (_lastHighlightedTarget != null)
                _lastHighlightedTarget.SetInteractionIndicator(false);

            if (nearby != null)
                nearby.SetInteractionIndicator(true);

            _lastHighlightedTarget = nearby;
        }
    }
    private SuspectHandler FindNearbySuspect()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            InteractRadius
        );

        foreach (var hit in hits)
        {
            SuspectHandler suspect = hit.GetComponentInParent<SuspectHandler>();
            if (suspect != null)
                return suspect;
        }

        return null;
    }

}

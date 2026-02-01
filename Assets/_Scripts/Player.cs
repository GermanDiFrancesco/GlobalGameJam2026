using UnityEngine;

public class Player : MonoBehaviour
{
    private const float MoveSpeed = 20f;
    private const float InteractHoldTime = 1f;
    private const float InteractRadius = 1.2f;

    private Rigidbody2D _rigidbody2D;
    private Vector3 _moveDir;

    private float _interactTimer;
    private SuspectHandler _currentTarget;
    private SuspectHandler _lastHighlightedTarget;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleMovementInput();
        UpdateInteractionTarget();
        HandleInteraction();
    }

    private void FixedUpdate()
    {
        _rigidbody2D.linearVelocity = _moveDir * MoveSpeed;
    }

    // ================= MOVEMENT =================

    private void HandleMovementInput()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY = +1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = +1f;

        _moveDir = new Vector3(moveX, moveY).normalized;
    }

    // ================= INTERACTION =================

    private void HandleInteraction()
    {
        if (Input.GetKey(KeyCode.E))
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

        if (Input.GetKeyUp(KeyCode.E))
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
            SuspectHandler suspect = hit.GetComponent<SuspectHandler>();
            if (suspect != null)
                return suspect;
        }

        return null;
    }

}

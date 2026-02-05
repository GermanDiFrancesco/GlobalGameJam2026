using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [Header("Target")] [SerializeField] private Transform _target; // El jugador

    [Header("Follow Settings")] [SerializeField]
    private float _smoothSpeed = 5f; // Suavidad del seguimiento

    [SerializeField] private Vector3 _offset = new Vector3(0, 0, -10); // Offset de la cámara

    [Header("Map Bounds")] [SerializeField]
    private float _minX = -10f; // Límite izquierdo

    [SerializeField] private float _maxX = 10f; // Límite derecho

    private void LateUpdate()
    {
        if (_target == null) return;

        // Calcular posición deseada (solo X sigue al jugador)
        float targetX = _target.position.x + _offset.x;
        float clampedX = Mathf.Clamp(targetX, _minX, _maxX);

        // Mantener Y y Z fijos
        Vector3 desiredPosition = new Vector3(
            clampedX,
            transform.position.y, // Y no cambia
            _offset.z // Z fijo (profundidad)
        );

        // Movimiento suave (lerp)
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            _smoothSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;
    }
}
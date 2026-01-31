using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Npc : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float maxDistanceFromSpawn = 5f;
    
    [Header("Configuración de Tiempos")]
    [SerializeField] private float maxIdleTime = 6f;  // Mínimo siempre es 3
    
    private Rigidbody2D _rigidbody2D;
    private Vector2 _spawnPosition;
    private Vector2 _moveDir;
    
    private enum NPCState { Idle, Moving }
    private NPCState _currentState;
    
    // Constante para el tiempo mínimo idle
    private const float MinIdleTime = 3f;
    
    // Propiedad calculada: tiempo máximo de movimiento basado en la distancia máxima
    //TODO: Dejarlo constante
    private const float MaxMoveTime = 6f;
    
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spawnPosition = transform.position;
    }

    private void Start()
    {
        StartCoroutine(StateMachine());
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

    private void FixedUpdate()
    {
        _rigidbody2D.velocity = _moveDir * moveSpeed;
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
}

using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new(0, 0, -10);

    [Header("Tutorial Bounds")]
    [SerializeField] private Vector3 tutorialMin;
    [SerializeField] private Vector3 tutorialMax;

    [Header("Gameplay Bounds")]
    [SerializeField] private Vector3 gameplayMin;
    [SerializeField] private Vector3 gameplayMax;

    private float currentMinX;
    private float currentMaxX;

    private void Awake() => SetTutorialBounds();

    private void LateUpdate()
    {
        if (target == null) return;

        float targetX = target.position.x + offset.x;
        float clampedX = Mathf.Clamp(targetX, currentMinX, currentMaxX);

        Vector3 desiredPosition = new Vector3(
            clampedX,
            transform.position.y,
            offset.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    public void SetTutorialBounds()
    {
        currentMinX = tutorialMin.x;
        currentMaxX = tutorialMax.x;
    }
    public void SetGameplayBounds()
    {
        currentMinX = gameplayMin.x;
        currentMaxX = gameplayMax.x;
    }

    public void SetCustomBounds(float minX, float maxX)
    {
        currentMinX = minX;
        currentMaxX = maxX;
    }
}
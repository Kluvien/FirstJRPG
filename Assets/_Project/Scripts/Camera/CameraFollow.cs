using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Map Bounds")]
    [SerializeField] private SpriteRenderer mapBoundsSprite;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        if (mapBoundsSprite != null)
        {
            targetPosition = ClampCameraPosition(targetPosition);
        }

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    private Vector3 ClampCameraPosition(Vector3 targetPosition)
    {
        Bounds bounds = mapBoundsSprite.bounds;

        float cameraHeight = cam.orthographicSize;
        float cameraWidth = cameraHeight * cam.aspect;

        float minX = bounds.min.x + cameraWidth;
        float maxX = bounds.max.x - cameraWidth;
        float minY = bounds.min.y + cameraHeight;
        float maxY = bounds.max.y - cameraHeight;

        float clampedX = targetPosition.x;
        float clampedY = targetPosition.y;

        if (minX <= maxX)
        {
            clampedX = Mathf.Clamp(targetPosition.x, minX, maxX);
        }
        else
        {
            clampedX = bounds.center.x;
        }

        if (minY <= maxY)
        {
            clampedY = Mathf.Clamp(targetPosition.y, minY, maxY);
        }
        else
        {
            clampedY = bounds.center.y;
        }

        return new Vector3(clampedX, clampedY, targetPosition.z);
    }
}
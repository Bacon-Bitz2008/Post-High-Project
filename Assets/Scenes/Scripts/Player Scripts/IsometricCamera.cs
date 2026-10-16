using System.Collections.Generic;
using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Normal Camera")]
    public float height = 12f;
    public float distance = 12f;
    public float Angle = 12f;

    [Header("Camera Movement")]
    public float followSpeed = 10f;
    public float rotationSpeed = 10f;

    [Header("Wall Collision")]
    public LayerMask wallLayers;
    public float pivotHeight = 1.5f;
    public float cameraRadius = 0.4f;
    public float wallBuffer = 0.3f;
    public float minCameraDistance = 2f;

    [Header("Target Lock")]
    public List<Transform> targets = new List<Transform>();
    public Transform target;
    public bool isLocked = false;

    public float lockedDistance = 10f;
    public float lockedHeight = 10f;

    [Range(0f, 1f)]
    public float targetBias = 0.25f;

    [Header("Player Facing")]
    public float playerRotationSpeed = 10f;

    void LateUpdate()
    {
        if (player == null)
            return;

        if (isLocked && target != null)
        {
            LockOnTarget();
        }
        else
        {
            NormalCamera();
        }
    }

    // Stops the camera from going through walls
    Vector3 AdjustForWalls(Vector3 desiredPosition)
    {
        Vector3 pivot = player.position + Vector3.up * pivotHeight;
        Vector3 direction = desiredPosition - pivot;
        float maxDistance = direction.magnitude;

        if (maxDistance < 0.01f)
            return desiredPosition;

        direction.Normalize();

        RaycastHit hit;

        if (Physics.SphereCast(pivot, cameraRadius, direction, out hit, maxDistance, wallLayers, QueryTriggerInteraction.Ignore))
        {
            float safeDistance = Mathf.Max(hit.distance - wallBuffer, minCameraDistance);
            return pivot + direction * safeDistance;
        }

        return desiredPosition;
    }

    void NormalCamera()
    {
        Vector3 targetPosition = player.position;

        targetPosition.x += Angle;
        targetPosition.y += height;
        targetPosition.z -= distance;

        targetPosition = AdjustForWalls(targetPosition);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        Quaternion targetRotation = Quaternion.LookRotation(
            player.position - transform.position
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void LockOnTarget()
    {
        Vector3 targetPosition = player.position;

        targetPosition.x += Angle;
        targetPosition.y += lockedHeight;
        targetPosition.z -= lockedDistance;

        targetPosition = AdjustForWalls(targetPosition);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        Vector3 lookPoint = Vector3.Lerp(
            player.position,
            target.position,
            targetBias
        );

        Quaternion targetRotation = Quaternion.LookRotation(
            lookPoint - transform.position
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        FaceTarget();
    }

    void FaceTarget()
    {
        if (target == null)
            return;

        Vector3 direction = target.position - player.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        player.rotation = Quaternion.Slerp(
            player.rotation,
            targetRotation,
            playerRotationSpeed * Time.deltaTime
        );
    }

    // Finds the closest target from the list.
    Transform FindClosestTarget()
    {
        if (targets == null || targets.Count == 0)
            return null;

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform possibleTarget in targets)
        {
            if (possibleTarget == null)
                continue;

            float distanceToTarget = Vector3.Distance(
                player.position,
                possibleTarget.position
            );

            if (distanceToTarget < closestDistance)
            {
                closestDistance = distanceToTarget;
                closestTarget = possibleTarget;
            }
        }

        return closestTarget;
    }

    // Lock onto the closest target.
    public void LockOn()
    {
        if (player == null)
            return;

        Transform closestTarget = FindClosestTarget();

        if (closestTarget == null)
            return;

        target = closestTarget;
        isLocked = true;
    }

    public void Unlock()
    {
        isLocked = false;
        target = null;
    }
}
using UnityEngine;

public class EnemyLookAt : MonoBehaviour
{
    [Header("Turning")]
    public float turnSpeed = 5f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stopDistance = 2f;      // how close the enemy gets to the player
    public float resumeBuffer = 0.3f;    // player must move this much past stopDistance before the enemy walks again

    [Header("Animation")]
    public Animator animator;
    public string walkParameter = "IsWalking";

    private Transform target;
    private Quaternion originalRotation;
    private bool isWalking = false;

    void Start()
    {
        originalRotation = transform.rotation;

        // If you didn't drag an Animator in, look for one on this object or its children
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ClearTarget()
    {
        target = null;
    }

    void Update()
    {
        Quaternion desiredRotation = originalRotation;
        bool shouldWalk = false;

        if (target != null)
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0f; // ignore height differences

            float distanceToPlayer = direction.magnitude;

            if (direction.sqrMagnitude > 0.001f)
            {
                desiredRotation = Quaternion.LookRotation(direction);
            }

            // Decide whether to walk. The buffer stops the enemy from
            // flickering between walking and idle right at the stop distance.
            if (isWalking)
            {
                shouldWalk = distanceToPlayer > stopDistance;
            }
            else
            {
                shouldWalk = distanceToPlayer > stopDistance + resumeBuffer;
            }

            if (shouldWalk)
            {
                transform.position += direction.normalized * moveSpeed * Time.deltaTime;
            }
        }

        isWalking = shouldWalk;

        // Smoothly turn toward the desired rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);

        // Tell the Animator whether we're walking
        if (animator != null)
        {
            animator.SetBool(walkParameter, isWalking);
        }
    }
}
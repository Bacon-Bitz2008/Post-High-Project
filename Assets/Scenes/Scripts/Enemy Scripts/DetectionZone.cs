using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    public EnemyLookAt enemy; // drag your enemy here in the Inspector

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            enemy.SetTarget(other.transform);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            enemy.ClearTarget();
    }
}
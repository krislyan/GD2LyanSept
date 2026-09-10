using UnityEngine;
using Ludocore;

public class HealthRefillZone : MonoBehaviour
{
    [SerializeField] private int healSpeed = 1;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.Heal(healSpeed);
        }
    }
}
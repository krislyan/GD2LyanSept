using UnityEngine;

namespace Ludocore
{
    public class EnemyController : MonoBehaviour
    {
        //==================== CONFIG =====================
        [Header("Modules")]
        [Tooltip("Motor used for NavMesh-based movement.")]
        [SerializeField] private NavMeshMotor motor;
        [Tooltip("Finds the highest-priority target in range, with the Core as fallback. Replaces the old Sensor + priority-tag find-loop combo.")]
        [SerializeField] private PriorityTargeting targeting;
        [Tooltip("Spawner that emits a projectile each attack tick. Its own cooldown controls attack rate.")]
        [SerializeField] private Spawner attackSpawner;

        [Header("Behavior")]
        [Tooltip("Distance at which the enemy stops moving and starts attacking.")]
        [Min(0f)]
        [SerializeField] private float attackRange = 4f;

        //==================== STATE =====================
        [Header("Debug")]
        [ReadOnly, SerializeField] private string currentBehavior;

        //==================== LIFECYCLE =====================
        private void Update()
        {
            if (!targeting.HasTarget)
            {
                motor.Stop();
                currentBehavior = "Idle";
                return;
            }

            if (targeting.Distance > attackRange)
            {
                motor.MoveTo(targeting.Target.position);
                currentBehavior = "Seek";
            }
            else
            {
                motor.Stop();
                FaceFlat(targeting.Target.position);
                attackSpawner.Spawn();
                currentBehavior = "Attack";
            }
        }

        //==================== PRIVATE =====================
        private void FaceFlat(Vector3 worldPos)
        {
            worldPos.y = transform.position.y;
            if (worldPos != transform.position) transform.LookAt(worldPos);
        }
    }
}
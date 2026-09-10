using UnityEngine;

/// <summary>
/// Attach to a static mesh GameObject.
/// When the player walks within triggerDistance, spawns 3 prefabs
/// at random positions within spawnRadius around this object.
/// Spawns only once.
/// </summary>
public class ProximitySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("The prefab to spawn. Spawns 3 copies.")]
    public GameObject prefabToSpawn;

    [Header("Proximity")]
    [Tooltip("How close the player must get to trigger the spawn.")]
    public float triggerDistance = 5f;

    [Tooltip("Tag on the player GameObject.")]
    public string playerTag = "Player";

    [Header("Spawn Area")]
    [Tooltip("Radius around this object within which prefabs are randomly placed.")]
    public float spawnRadius = 3f;

    [Tooltip("Offset the spawn height relative to this object's position.")]
    public float spawnHeightOffset = 0f;

    [Tooltip("Number of prefabs to spawn.")]
    public int spawnCount = 3;

    private Transform _player;
    private bool _hasSpawned = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag(playerTag);
        if (playerObj != null)
            _player = playerObj.transform;
        else
            Debug.LogWarning($"[ProximitySpawner] No GameObject found with tag '{playerTag}'.");
    }

    private void Update()
    {
        if (_hasSpawned || _player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        if (dist <= triggerDistance)
        {
            SpawnPrefabs();
            _hasSpawned = true;
        }
    }

    private void SpawnPrefabs()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"[ProximitySpawner] No prefab assigned on {gameObject.name}.");
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            Debug.Log($"[ProximitySpawner] Spawned '{prefabToSpawn.name}' at {spawnPos}");
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Random point inside a circle, then placed at this object's height
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return new Vector3(
            transform.position.x + randomCircle.x,
            transform.position.y + spawnHeightOffset,
            transform.position.z + randomCircle.y
        );
    }

    private void OnDrawGizmosSelected()
    {
        // Trigger radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);

        // Spawn area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using Ludocore;

[RequireComponent(typeof(BuildingSite))]
public class EncounterSpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    [Tooltip("The enemy prefab to spawn.")]
    public GameObject enemyPrefab;

    [Tooltip("How many enemies to spawn.")]
    public int enemyCount = 3;

    [Tooltip("Radius around this object within which enemies are randomly placed.")]
    public float enemySpawnRadius = 4f;

    [Tooltip("Y height offset for enemy spawns relative to this object.")]
    public float enemySpawnHeightOffset = 0f;

    [Header("Magic Circle Settings")]
    [Tooltip("The magic circle effect prefab.")]
    public GameObject magicCirclePrefab;

    [Tooltip("Y height at which the magic circle spawns. X and Z match this object.")]
    public float magicCircleHeightY = 0f;

    [Tooltip("Rotation of the magic circle.")]
    public Vector3 magicCircleRotation;

    [Header("Atmosphere / Lighting")]
    [Tooltip("Ссылка на объект с FightAtmosphereController")]
    public FightAtmosphereController atmosphereController;

    [Header("Trigger Settings")]
    [Tooltip("If set, the encounter only triggers when THIS specific building tier is built.")]
    public BuildingData triggerOnData;

    [Tooltip("If true, the encounter can only trigger once ever.")]
    public bool triggerOnce = true;

    // Внутренние поля
    private BuildingSite _buildingSite;
    private GameObject _magicCircleInstance;
    private List<GameObject> _spawnedEnemies = new List<GameObject>();
    private bool _hasTriggered = false;
    private bool _isEncounterActive = false;

    public bool IsActive => _magicCircleInstance != null;
    public bool IsCleared { get; private set; }

    public event Action OnEncounterStarted;
    public event Action OnEncounterCleared;

    private void Awake()
    {
        _buildingSite = GetComponent<BuildingSite>();

        if (atmosphereController == null)
            atmosphereController = FindFirstObjectByType<FightAtmosphereController>();
    }

    private void OnEnable()
    {
        _buildingSite.OnBuilt += HandleOnBuilt;
    }

    private void OnDisable()
    {
        _buildingSite.OnBuilt -= HandleOnBuilt;
    }

    private void Update()
    {
        if (_isEncounterActive)
        {
            CheckEnemiesDefeated();
        }
    }

    private void HandleOnBuilt(BuildingData previous, BuildingData current)
    {
        if (triggerOnce && _hasTriggered) return;
        if (triggerOnData != null && current != triggerOnData) return;

        TriggerEncounter();
        _hasTriggered = true;
    }

    private void TriggerEncounter()
    {
        SpawnMagicCircle();
        SpawnEnemies();

        _isEncounterActive = true;

        if (atmosphereController != null)
        {
            atmosphereController.StartFightNight(_spawnedEnemies.Count);
        }

        OnEncounterStarted?.Invoke();
    }

    private void SpawnMagicCircle()
    {
        if (magicCirclePrefab == null)
        {
            Debug.LogWarning($"[EncounterSpawner] No magic circle prefab assigned on {gameObject.name}.");
            return;
        }

        Vector3 spawnPos = new Vector3(transform.position.x, magicCircleHeightY, transform.position.z);
        Quaternion rotation = Quaternion.Euler(magicCircleRotation);
        _magicCircleInstance = Instantiate(magicCirclePrefab, spawnPos, rotation);
    }

    private void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning($"[EncounterSpawner] No enemy prefab assigned on {gameObject.name}.");
            return;
        }

        _spawnedEnemies.Clear();

        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * enemySpawnRadius;
            Vector3 spawnPos = new Vector3(
                transform.position.x + randomCircle.x,
                transform.position.y + enemySpawnHeightOffset,
                transform.position.z + randomCircle.y
            );

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            _spawnedEnemies.Add(enemy);
        }
    }

    private void CheckEnemiesDefeated()
    {
        // Очищаем из списка мертвых и выключенных врагов
        _spawnedEnemies.RemoveAll(e => e == null || !e.activeInHierarchy);

        if (_spawnedEnemies.Count == 0)
        {
            _isEncounterActive = false;

            if (_magicCircleInstance != null)
            {
                Destroy(_magicCircleInstance);
                _magicCircleInstance = null;
            }

            if (atmosphereController != null)
            {
                atmosphereController.EndFightDay();
            }

            IsCleared = true;
            OnEncounterCleared?.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemySpawnRadius);

        Vector3 circlePos = new Vector3(transform.position.x, magicCircleHeightY, transform.position.z);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(circlePos, 0.2f);
        Gizmos.DrawLine(transform.position, circlePos);
    }
}
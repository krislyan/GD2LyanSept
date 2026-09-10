using UnityEngine;

namespace Ludocore
{
    /// <summary>
    /// Finds the highest-priority tagged target in a Sensor, falling back to a
    /// designated Core transform. If the Sensor is unconfigured or empty, 
    /// it automatically falls back to direct detection so enemies never freeze.
    /// </summary>
    public class PriorityTargeting : Targeting
    {
        //==================== CONFIG =====================
        [Header("Config")]
        [Tooltip("Sensor that detects candidate targets.")]
        [SerializeField] private Sensor sensor;

        [Tooltip("Priority order, top = highest. The sensor must also be filtered to detect these tags.")]
        [SerializeField] private string[] priorityTags = { "Player", "Worker", "Building" };

        [Tooltip("Tag of the fallback target — found by tag, used when nothing in priorityTags is in range.")]
        [SerializeField] private string coreTag = "Core";

        [Header("Sensor Fallback (Auto-Detect)")]
        [Tooltip("Радиус прямого поиска игрока, если Trigger Sensor пуст или сбит")]
        [SerializeField] private float directDetectRadius = 25f;

        //==================== STATE =====================
        private Transform _target;
        private Transform _core;
        private float _distance = float.MaxValue;
        private Transform _playerFallback;

        public override bool      HasTarget => _target != null;
        public override Transform Target    => _target;
        public override float     Distance  => _distance;

        //==================== LIFECYCLE =====================
        private void Awake()
        {
            // Автоподключение сенсора, если ссылка в инспекторе была пустой
            if (sensor == null)
                sensor = GetComponent<Sensor>() ?? GetComponentInChildren<Sensor>();

            FindCore();
            FindPlayer();
        }

        private void Update() => Scan();

        //==================== PRIVATE =====================
        private void Scan()
        {
            _target = null;
            _distance = float.MaxValue;

            // 1. Штатное сканирование через родной Sensor Ludocore
            if (sensor != null && sensor.Signals != null && sensor.Signals.Count > 0)
            {
                var signals = sensor.Signals;

                for (int p = 0; p < priorityTags.Length; p++)
                {
                    Transform best = null;
                    float bestDistance = float.MaxValue;

                    for (int i = 0; i < signals.Count; i++)
                    {
                        var obj = signals[i].Object;
                        if (!obj) continue;
                        if (!obj.CompareTag(priorityTags[p])) continue;

                        if (signals[i].Distance < bestDistance)
                        {
                            best = obj.transform;
                            bestDistance = signals[i].Distance;
                        }
                    }

                    if (best != null)
                    {
                        _target = best;
                        _distance = bestDistance;
                        return;
                    }
                }
            }

            // 2. Страховка: если Trigger Sensor пуст (0 тегов в инспекторе) — берем игрока напрямую
            if (_playerFallback == null) FindPlayer();

            if (_playerFallback != null)
            {
                float distToPlayer = Vector3.Distance(transform.position, _playerFallback.position);
                if (distToPlayer <= directDetectRadius)
                {
                    _target = _playerFallback;
                    _distance = distToPlayer;
                    return;
                }
            }

            // 3. Запасная цель: святилище (Core)
            if (_core == null) FindCore();

            if (_core != null)
            {
                _target = _core;
                _distance = Vector3.Distance(transform.position, _core.position);
            }
        }

        private void FindCore()
        {
            if (!string.IsNullOrEmpty(coreTag))
            {
                var hub = GameObject.FindWithTag(coreTag);
                if (hub != null) _core = hub.transform;
            }

            // Поиск по имени, если тег Core забыли выставить
            if (_core == null)
            {
                var fallbackHub = GameObject.Find("Shrine") ?? GameObject.Find("Totem");
                if (fallbackHub != null) _core = fallbackHub.transform;
            }
        }

        private void FindPlayer()
        {
            var p = GameObject.FindWithTag("Player") ?? GameObject.Find("PlayerArmature");
            if (p != null) _playerFallback = p.transform;
        }
    }
}
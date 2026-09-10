using UnityEngine;
using UnityEngine.UI;
using Ludocore;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private HealthSystem healthSystem;
    private Camera mainCamera;

    private void Awake()
    {
        if (slider == null)
            slider = GetComponentInChildren<Slider>();

        if (healthSystem == null)
            healthSystem = GetComponentInParent<HealthSystem>();
    }

    private void Start()
    {
        mainCamera = Camera.main;

        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
        }

        UpdateHealthBar();
    }

    private void OnEnable()
    {
        if (healthSystem != null)
            healthSystem.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        if (healthSystem != null)
            healthSystem.OnHealthChanged -= UpdateHealthBar;
    }

    private void LateUpdate()
    {
        // Поворот лицом к камере игрока
        if (mainCamera != null)
            transform.rotation = mainCamera.transform.rotation;
    }

    private void UpdateHealthBar()
    {
        if (slider != null && healthSystem != null)
        {
            slider.value = healthSystem.HealthRatio;
        }
    }
}
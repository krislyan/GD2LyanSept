using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ludocore;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private HealthSystem health;   // drag the Player's HealthSystem here
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text hpText;

    private void OnEnable()
    {
        if (health != null) health.OnHealthChanged += Refresh;
    }

    private void OnDisable()
    {
        if (health != null) health.OnHealthChanged -= Refresh;
    }

    private void Start() => Refresh();              // initial sync

    private void Refresh()
    {
        if (health == null) return;

        slider.maxValue = health.MaxHealth;
        slider.value    = health.CurrentHealth;

        int current = Mathf.CeilToInt(health.CurrentHealth);
        int max     = Mathf.CeilToInt(health.MaxHealth);
        hpText.text = current + " / " + max;
    }
}
using System.Collections;
using UnityEngine;

public class FightAtmosphereController : MonoBehaviour
{
    [Header("Sun / Directional Light")]
    [SerializeField] private Light sunLight;
    
    [Header("Day Preset")]
    [SerializeField] private Color dayColor = new Color(1f, 0.95f, 0.85f);
    [SerializeField] private float dayIntensity = 1.2f;
    [SerializeField] private Vector3 dayRotation = new Vector3(50f, -30f, 0f);

    [Header("Night Preset")]
    [SerializeField] private Color nightColor = new Color(0.2f, 0.25f, 0.5f);
    [SerializeField] private float nightIntensity = 0.3f;
    [SerializeField] private Vector3 nightRotation = new Vector3(170f, -30f, 0f);

    [Header("Settings")]
    [SerializeField] private float transitionDuration = 2.5f;

    private Coroutine transitionRoutine;
    private int aliveEnemiesCount = 0;

    private void Start()
    {
        // Устанавливаем день при старте сцены
        ApplyPresetImmediate(dayColor, dayIntensity, dayRotation);
    }

    // Вызывается при старте волны / активации святилища
    public void StartFightNight(int enemyCount)
    {
        aliveEnemiesCount = enemyCount;
        SwitchAtmosphere(nightColor, nightIntensity, nightRotation);
    }

    // Вызывается при смерти каждого скелета
    public void OnEnemyKilled()
    {
        aliveEnemiesCount--;
        if (aliveEnemiesCount <= 0)
        {
            aliveEnemiesCount = 0;
            EndFightDay();
        }
    }

    // Принудительный возврат дня
    public void EndFightDay()
    {
        SwitchAtmosphere(dayColor, dayIntensity, dayRotation);
    }

    private void SwitchAtmosphere(Color targetColor, float targetIntensity, Vector3 targetRot)
    {
        if (transitionRoutine != null) StopCoroutine(transitionRoutine);
        transitionRoutine = StartCoroutine(TransitionRoutine(targetColor, targetIntensity, targetRot));
    }

    private IEnumerator TransitionRoutine(Color targetColor, float targetIntensity, Vector3 targetRot)
    {
        if (sunLight == null) yield break;

        Color startColor = sunLight.color;
        float startIntensity = sunLight.intensity;
        Quaternion startRot = sunLight.transform.rotation;
        Quaternion endRot = Quaternion.Euler(targetRot);

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);

            sunLight.color = Color.Lerp(startColor, targetColor, t);
            sunLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
            sunLight.transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        sunLight.color = targetColor;
        sunLight.intensity = targetIntensity;
        sunLight.transform.rotation = endRot;
    }

    private void ApplyPresetImmediate(Color col, float intensity, Vector3 rot)
    {
        if (sunLight == null) return;
        sunLight.color = col;
        sunLight.intensity = intensity;
        sunLight.transform.rotation = Quaternion.Euler(rot);
    }
}
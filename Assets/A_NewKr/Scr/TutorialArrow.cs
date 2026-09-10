using UnityEngine;

public class TutorialArrow : MonoBehaviour
{
    [SerializeField] private GameObject root;       // видимая часть стрелки
    [SerializeField] private float heightAbove = 2.5f;
    [SerializeField] private float bobHeight = 0.3f;
    [SerializeField] private float bobSpeed = 3f;

    private Transform target;

    private void Awake() => Hide();

    public void PointAt(Transform newTarget)
    {
        target = newTarget;
        root.SetActive(true);
    }

    public void Hide()
    {
        target = null;
        root.SetActive(false);
    }

    private void Update()
    {
        if (target == null) return;
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = target.position + Vector3.up * (heightAbove + bob);
    }
}

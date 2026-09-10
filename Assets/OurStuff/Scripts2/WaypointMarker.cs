using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A billboarded world-space icon used to mark quest objectives (villager, shrines).
/// Works with EITHER a world-space Canvas + UI Image, OR a SpriteRenderer —
/// assign whichever one your marker prefab uses and leave the other empty.
/// </summary>
public class WaypointMarker : MonoBehaviour
{
    public enum MarkerState { Arrow, Danger, Cleared }

    [Header("Icons")]
    public Sprite arrowIcon;
    public Sprite dangerIcon;   // skull, shown while the shrine's enemies are active
    public Sprite clearedIcon;  // checkmark, shown briefly once the shrine is cleared

    [Header("Renderer (assign ONE of these)")]
    [Tooltip("If this marker is a world-space Canvas + UI Image.")]
    public Image uiImage;
    [Tooltip("If this marker is a plain SpriteRenderer instead.")]
    public SpriteRenderer spriteRenderer;

    [Header("Positioning")]
    [Tooltip("The world object this marker should hover above.")]
    public Transform followTarget;
    public Vector3 worldOffset = new Vector3(0f, 2.5f, 0f);

    [Header("Billboard")]
    public bool billboard = true;
    [Tooltip("If true, only rotates around Y so the icon never tilts up/down.")]
    public bool lockVerticalTilt = true;

    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (followTarget != null)
            transform.position = followTarget.position + worldOffset;

        if (billboard)
            FaceCamera();
    }

    private void FaceCamera()
    {
        if (_cam == null)
        {
            _cam = Camera.main;
            if (_cam == null) return;
        }

        Vector3 dir = transform.position - _cam.transform.position;
        if (lockVerticalTilt) dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        transform.rotation = Quaternion.LookRotation(dir);
    }

    public void SetState(MarkerState state)
    {
        Sprite icon = state switch
        {
            MarkerState.Arrow => arrowIcon,
            MarkerState.Danger => dangerIcon,
            MarkerState.Cleared => clearedIcon,
            _ => arrowIcon
        };

        if (uiImage != null) uiImage.sprite = icon;
        if (spriteRenderer != null) spriteRenderer.sprite = icon;
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
        if (target != null)
            transform.position = target.position + worldOffset;
    }
}

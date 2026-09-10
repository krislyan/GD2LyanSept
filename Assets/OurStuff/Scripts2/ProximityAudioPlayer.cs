using UnityEngine;

/// <summary>
/// Plays an audio clip when the player enters this object's trigger collider.
/// Standalone — doesn't depend on the quest or dialogue systems.
/// Requires a trigger Collider on this object (or a child).
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ProximityAudioPlayer : MonoBehaviour
{
    [Header("Trigger")]
    public string playerTag = "Player";

    [Header("Audio")]
    public AudioClip clip;

    [Header("Repeat Behavior")]
    [Tooltip("If true, only plays the very first time the player enters. If false, plays every time they enter the trigger.")]
    public bool playOnce = true;

    [Tooltip("Minimum seconds between plays when playOnce is false (prevents spamming if the player lingers on the trigger edge).")]
    public float cooldownSeconds = 1f;

    private AudioSource _audioSource;
    private bool _hasPlayed;
    private float _lastPlayTime = -999f;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (playOnce && _hasPlayed) return;
        if (Time.time - _lastPlayTime < cooldownSeconds) return;

        Play();
    }

    private void Play()
    {
        if (clip == null)
        {
            Debug.LogWarning($"[ProximityAudioPlayer] No AudioClip assigned on {gameObject.name}.");
            return;
        }

        _audioSource.clip = clip;
        _audioSource.Play();

        _hasPlayed = true;
        _lastPlayTime = Time.time;
    }
}

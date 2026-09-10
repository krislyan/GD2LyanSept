using UnityEngine;

/// <summary>
/// Attach this script to any static mesh GameObject.
/// Plays an AudioClip once when the player walks within triggerDistance.
/// Resets after the player walks back out, ready to play again.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class InteractableAudio : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("The audio clip to play when the player gets close.")]
    public AudioClip audioClip;

    [Header("Proximity Settings")]
    [Tooltip("Distance at which the audio triggers.")]
    public float triggerDistance = 3f;

    [Tooltip("Tag on the player GameObject.")]
    public string playerTag = "Player";

    private AudioSource _audioSource;
    private Transform _player;
    private static bool _hasPlayed = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;

        if (audioClip != null)
            _audioSource.clip = audioClip;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag(playerTag);
        if (playerObj != null)
            _player = playerObj.transform;
        else
            Debug.LogWarning($"[InteractableAudio] No GameObject found with tag '{playerTag}'.");
    }

    private void Update()
    {
        if (_player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        bool playerIsClose = dist <= triggerDistance;

        if (playerIsClose && !_hasPlayed)
        {
            PlayAudio();
            _hasPlayed = true;
        }

    }

    private void PlayAudio()
    {
        if (audioClip == null)
        {
            Debug.LogWarning($"[InteractableAudio] No AudioClip assigned on {gameObject.name}.");
            return;
        }

        _audioSource.PlayOneShot(audioClip);
        Debug.Log($"[InteractableAudio] Playing '{audioClip.name}' on {gameObject.name}.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}
using UnityEngine;
using System.Collections;

/// <summary>
/// Attach to any GameObject.
/// Plays one of three AudioClips in sequence (or randomly) with a delay between each.
/// Loops forever.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioSequencer : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip clip1;
    public AudioClip clip2;
    public AudioClip clip3;

    [Header("Timing")]
    [Tooltip("Delay in seconds between each audio clip.")]
    public float delayBetweenClips = 2f;

    [Header("Playback Order")]
    [Tooltip("If true, picks a random clip each time. If false, plays them in order 1 > 2 > 3 > 1...")]
    public bool randomOrder = false;

    private AudioSource _audioSource;
    private AudioClip[] _clips;
    private int _currentIndex = 0;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
    }

    private void Start()
    {
        _clips = new AudioClip[] { clip1, clip2, clip3 };
        StartCoroutine(PlayLoop());
    }

    private IEnumerator PlayLoop()
    {
        while (true)
        {
            AudioClip clipToPlay = GetNextClip();

            if (clipToPlay != null)
            {
                _audioSource.PlayOneShot(clipToPlay);
                // Wait for the clip to finish, then add the extra delay
                yield return new WaitForSeconds(clipToPlay.length + delayBetweenClips);
            }
            else
            {
                // No clip assigned in this slot, just wait the delay and move on
                yield return new WaitForSeconds(delayBetweenClips);
            }
        }
    }

    private AudioClip GetNextClip()
    {
        if (randomOrder)
        {
            return _clips[Random.Range(0, _clips.Length)];
        }
        else
        {
            AudioClip clip = _clips[_currentIndex];
            _currentIndex = (_currentIndex + 1) % _clips.Length;
            return clip;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// A simple on-screen chat box: shows one line of text at a time, optionally
/// plays an audio clip alongside it, and auto-advances to the next line once
/// the clip (or fallback timer) finishes. Call PlayLine/PlayLines from anywhere
/// (QuestGiver, shrine triggers, NPCs, etc.) to use it.
///
/// IMPORTANT: visibility is controlled via a CanvasGroup (alpha/raycasts), NOT
/// GameObject.SetActive(). This is deliberate — if this script's own GameObject
/// were the one being deactivated, any running coroutine on it would be killed
/// immediately, breaking playback. Using CanvasGroup means it's safe for
/// DialogueBox to live on the same object as the visual panel.
/// </summary>
public class DialogueBox : MonoBehaviour
{
    public static DialogueBox Instance { get; private set; }

    [Header("UI")]
    [Tooltip("CanvasGroup on the panel that should show/hide. Add a CanvasGroup component " +
             "to your panel object (Add Component > Canvas Group) and drag it here.")]
    public CanvasGroup panelCanvasGroup;
    public TextMeshProUGUI dialogueText;
    [Tooltip("Optional: shows who is speaking, e.g. 'Villager'. Leave empty if unused.")]
    public TextMeshProUGUI speakerNameText;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Timing")]
    [Tooltip("Default extra seconds to hold each line after its audio/timer finishes, before advancing. " +
             "Overridden per-line if that line's 'Pause After This Line' is >= 0.")]
    public float defaultGapBetweenLines = 0.3f;

    private Coroutine _playRoutine;

    private void Awake()
    {
        Instance = this;
        SetPanelVisible(false);
    }

    /// <summary>Play a single line.</summary>
    public void PlayLine(DialogueLine line, string speakerName = null)
    {
        PlayLines(new List<DialogueLine> { line }, speakerName);
    }

    /// <summary>Play a sequence of lines back-to-back, advancing automatically.</summary>
    public void PlayLines(List<DialogueLine> lines, string speakerName = null)
    {
        if (lines == null || lines.Count == 0)
        {
            Debug.LogWarning("[DialogueBox] PlayLines called with an empty list — nothing to show.");
            return;
        }

        Debug.Log($"[DialogueBox] PlayLines starting with {lines.Count} line(s). " +
                  $"panelCanvasGroup={(panelCanvasGroup != null ? panelCanvasGroup.name : "NULL")}, dialogueText={(dialogueText != null ? "OK" : "NULL")}, audioSource={(audioSource != null ? "OK" : "NULL")}");

        if (_playRoutine != null)
            StopCoroutine(_playRoutine);

        _playRoutine = StartCoroutine(PlaySequence(lines, speakerName));
    }

    /// <summary>Cuts off whatever is currently playing and hides the box immediately.</summary>
    public void Stop()
    {
        if (_playRoutine != null)
        {
            StopCoroutine(_playRoutine);
            _playRoutine = null;
        }

        if (audioSource != null)
            audioSource.Stop();

        SetPanelVisible(false);
    }

    private void SetPanelVisible(bool visible)
    {
        if (panelCanvasGroup == null) return;

        panelCanvasGroup.alpha = visible ? 1f : 0f;
        panelCanvasGroup.interactable = visible;
        panelCanvasGroup.blocksRaycasts = visible;
    }

    private IEnumerator PlaySequence(List<DialogueLine> lines, string speakerName)
    {
        SetPanelVisible(true);

        if (speakerNameText != null)
            speakerNameText.text = speakerName ?? string.Empty;

        foreach (DialogueLine line in lines)
        {
            if (dialogueText != null)
                dialogueText.text = line.text;

            float wait = line.displayDurationIfNoAudio;

            if (line.audioClip != null && audioSource != null)
            {
                audioSource.clip = line.audioClip;
                audioSource.Play();
                wait = line.audioClip.length;
                Debug.Log($"[DialogueBox] Line: \"{line.text}\" — playing clip '{line.audioClip.name}' ({wait:F2}s), audioSource.isPlaying={audioSource.isPlaying}");
            }
            else
            {
                Debug.Log($"[DialogueBox] Line: \"{line.text}\" — no audio clip, holding for {wait:F2}s (audioClip null? {line.audioClip == null}, audioSource null? {audioSource == null})");
            }

            float gap = line.pauseAfterThisLine >= 0f ? line.pauseAfterThisLine : defaultGapBetweenLines;
            yield return new WaitForSeconds(wait + gap);
        }

        SetPanelVisible(false);
        _playRoutine = null;
    }
}

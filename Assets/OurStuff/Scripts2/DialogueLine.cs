using UnityEngine;

/// <summary>
/// One line of dialogue: the text to show and the audio clip (if any) to play alongside it.
/// A list of these is what you hand to DialogueBox.PlayLines().
/// </summary>
[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 4)]
    public string text;

    [Tooltip("Optional. If assigned, the line stays on screen for the clip's length. " +
             "If left empty, it uses 'Display Duration If No Audio' instead.")]
    public AudioClip audioClip;

    [Tooltip("Only used when audioClip is empty — how long (seconds) this line stays on screen.")]
    public float displayDurationIfNoAudio = 2.5f;

    [Tooltip("Pause (seconds) after THIS line finishes before the next one starts. " +
             "Set to -1 to use DialogueBox's default gap instead of overriding it per-line.")]
    public float pauseAfterThisLine = -1f;
}

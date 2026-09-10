using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Attach to the villager NPC. Shows a waypoint arrow above their head until the
/// player walks into range, at which point the quest is handed to QuestManager
/// (optionally after a short delay) and the arrow disappears. Can also play a
/// line (or lines) of dialogue through a DialogueBox at the same time.
/// Requires a trigger Collider on this object (or a child) sized to the "give quest" range.
/// </summary>
[RequireComponent(typeof(Collider))]
public class QuestGiver : MonoBehaviour
{
    [Header("Interaction")]
    public string playerTag = "Player";

    [Tooltip("Seconds between the player entering range and the quest actually starting " +
             "(quest text appearing, waypoint moving to the first shrine). Set to 0 for instant.")]
    public float questGiveDelay = 0f;

    [Header("Waypoint")]
    [Tooltip("The marker floating above the villager, shown until the quest is taken.")]
    public WaypointMarker villagerWaypoint;
    public Vector3 waypointOffset = new Vector3(0f, 2.2f, 0f);

    [Header("Dialogue (optional)")]
    [Tooltip("Leave empty if you don't want the villager to say anything.")]
    public DialogueBox dialogueBox;
    [Tooltip("Shown as the speaker name in the dialogue box, if it has one.")]
    public string speakerName = "Villager";
    public List<DialogueLine> questDialogue = new List<DialogueLine>();

    private bool _questGiven;
    private bool _countdownStarted;

    private void Start()
    {
        if (villagerWaypoint != null)
        {
            villagerWaypoint.worldOffset = waypointOffset;
            villagerWaypoint.SetFollowTarget(transform);
            villagerWaypoint.SetState(WaypointMarker.MarkerState.Arrow);
            villagerWaypoint.SetVisible(true);
        }
    }

    private void GiveQuest()
    {
        if (_questGiven) return;
        _questGiven = true;

        if (QuestManager.Instance != null)
            QuestManager.Instance.StartQuest();
        else
            Debug.LogWarning("[QuestGiver] No QuestManager found in the scene.");

        if (villagerWaypoint != null)
            villagerWaypoint.SetVisible(false);

        if (dialogueBox != null && questDialogue.Count > 0)
        {
            Debug.Log($"[QuestGiver] Playing {questDialogue.Count} dialogue line(s) via {dialogueBox.name}.");
            dialogueBox.PlayLines(questDialogue, speakerName);
        }
        else
        {
            Debug.LogWarning($"[QuestGiver] Dialogue not played — dialogueBox null? {dialogueBox == null}, questDialogue count = {questDialogue.Count}");
        }
    }

    private IEnumerator GiveQuestAfterDelay()
    {
        yield return new WaitForSeconds(questGiveDelay);
        GiveQuest();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[QuestGiver] OnTriggerEnter fired. other={other.name}, tag={other.tag}, expecting tag={playerTag}");

        if (_questGiven || _countdownStarted) return;
        if (!other.CompareTag(playerTag)) return;

        _countdownStarted = true;

        if (questGiveDelay <= 0f)
            GiveQuest();
        else
            StartCoroutine(GiveQuestAfterDelay());
    }
}
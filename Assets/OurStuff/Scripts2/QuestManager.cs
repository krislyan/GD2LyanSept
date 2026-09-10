using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Drives the "Clear shrines of evil spirits" quest.
/// Tracks how many shrines have been cleared, exposes progress events for the UI,
/// and moves the single reusable waypoint marker between shrines as they
/// become the active objective: Arrow (not yet triggered) -> Danger/skull
/// (enemies active) -> Cleared/check (done) -> jumps to the next shrine.
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Quest Text")]
    public string questDescription = "Clear shrines of evil spirits";

    [Header("Shrines (in the order the player should visit them)")]
    public List<EncounterSpawner> shrinesInOrder = new List<EncounterSpawner>();

    [Header("Waypoint")]
    [Tooltip("The single marker that jumps between shrines as they become the active objective.")]
    public WaypointMarker shrineWaypoint;
    public Vector3 shrineWaypointOffset = new Vector3(0f, 3f, 0f);
    [Tooltip("How long the checkmark stays visible before the marker moves to the next shrine.")]
    public float clearedIconLingerSeconds = 1.5f;

    public int ClearedCount { get; private set; }
    public int TotalShrines => shrinesInOrder.Count;
    public bool QuestActive { get; private set; }
    public bool QuestComplete { get; private set; }

    private int _currentShrineIndex = -1;

    /// <summary>Fired whenever ClearedCount changes. Args: (cleared, total).</summary>
    public event Action<int, int> OnProgressChanged;
    /// <summary>Fired once when the final shrine is cleared.</summary>
    public event Action OnQuestComplete;
    /// <summary>Fired the moment the quest is handed out by the villager.</summary>
    public event Action OnQuestStarted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (shrineWaypoint != null)
            shrineWaypoint.SetVisible(false);
    }

    /// <summary>Call this from the villager's interaction script (QuestGiver).</summary>
    public void StartQuest()
    {
        if (QuestActive || QuestComplete) return;

        QuestActive = true;
        ClearedCount = 0;
        _currentShrineIndex = -1;

        OnQuestStarted?.Invoke();
        OnProgressChanged?.Invoke(ClearedCount, TotalShrines);

        ActivateNextShrine();
    }

    private void ActivateNextShrine()
    {
        _currentShrineIndex++;

        if (_currentShrineIndex >= shrinesInOrder.Count)
        {
            CompleteQuest();
            return;
        }

        EncounterSpawner shrine = shrinesInOrder[_currentShrineIndex];
        if (shrine == null)
        {
            ActivateNextShrine(); // skip missing/unassigned entries safely
            return;
        }

        shrine.OnEncounterStarted += HandleCurrentShrineEncounterStarted;
        shrine.OnEncounterCleared += HandleCurrentShrineCleared;

        if (shrineWaypoint != null)
        {
            shrineWaypoint.worldOffset = shrineWaypointOffset;
            shrineWaypoint.SetFollowTarget(shrine.transform);
            shrineWaypoint.SetState(WaypointMarker.MarkerState.Arrow);
            shrineWaypoint.SetVisible(true);
        }

        // Safety net: if this shrine was already triggered/cleared before the
        // quest was picked up (e.g. player built it before talking to the villager),
        // reflect that immediately instead of showing a stale arrow.
        if (shrine.IsCleared)
        {
            HandleCurrentShrineCleared();
        }
        else if (shrine.IsActive)
        {
            HandleCurrentShrineEncounterStarted();
        }
    }

    private void HandleCurrentShrineEncounterStarted()
    {
        if (shrineWaypoint != null)
            shrineWaypoint.SetState(WaypointMarker.MarkerState.Danger);
    }

    private void HandleCurrentShrineCleared()
    {
        EncounterSpawner shrine = shrinesInOrder[_currentShrineIndex];
        shrine.OnEncounterStarted -= HandleCurrentShrineEncounterStarted;
        shrine.OnEncounterCleared -= HandleCurrentShrineCleared;

        ClearedCount++;
        OnProgressChanged?.Invoke(ClearedCount, TotalShrines);

        if (shrineWaypoint != null)
            shrineWaypoint.SetState(WaypointMarker.MarkerState.Cleared);

        StartCoroutine(MoveToNextShrineAfterDelay());
    }

    private IEnumerator MoveToNextShrineAfterDelay()
    {
        yield return new WaitForSeconds(clearedIconLingerSeconds);

        if (shrineWaypoint != null)
            shrineWaypoint.SetVisible(false);

        ActivateNextShrine();
    }

    private void CompleteQuest()
    {
        QuestActive = false;
        QuestComplete = true;

        if (shrineWaypoint != null)
            shrineWaypoint.SetVisible(false);

        OnQuestComplete?.Invoke();
    }

    /// <summary>Handy for the UI: "Clear shrines of evil spirits 2/3".</summary>
    public string GetQuestLine()
    {
        return $"{questDescription} {ClearedCount}/{TotalShrines}";
    }
}

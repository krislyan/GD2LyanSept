using UnityEngine;
using TMPro;

/// <summary>
/// Displays the live quest line, e.g. "Clear shrines of evil spirits 1/3".
/// Hides its panel until the quest is actually accepted from the villager.
/// </summary>
public class QuestUIController : MonoBehaviour
{
    [Tooltip("The text element on your quest UI canvas.")]
    public TextMeshProUGUI questText;

    [Tooltip("The root panel to show/hide (can be the same object as questText, or its parent).")]
    public GameObject questPanel;

    [Tooltip("Optional: text shown briefly when the quest is fully complete.")]
    public string questCompleteLine = "Quest complete!";

    private void Start()
    {
        // Subscribe in Start (not Awake/OnEnable) so QuestManager.Instance is
        // guaranteed to already be set by its own Awake().
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestStarted += HandleQuestStarted;
            QuestManager.Instance.OnProgressChanged += HandleProgressChanged;
            QuestManager.Instance.OnQuestComplete += HandleQuestComplete;
        }
        else
        {
            Debug.LogWarning("[QuestUIController] No QuestManager found in the scene.");
        }

        if (questPanel != null)
            questPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestStarted -= HandleQuestStarted;
            QuestManager.Instance.OnProgressChanged -= HandleProgressChanged;
            QuestManager.Instance.OnQuestComplete -= HandleQuestComplete;
        }
    }

    private void HandleQuestStarted()
    {
        if (questPanel != null)
            questPanel.SetActive(true);

        RefreshText();
    }

    private void HandleProgressChanged(int cleared, int total)
    {
        RefreshText();
    }

    private void HandleQuestComplete()
    {
        if (questText != null)
            questText.text = questCompleteLine;
    }

    private void RefreshText()
    {
        if (questText != null && QuestManager.Instance != null)
            questText.text = QuestManager.Instance.GetQuestLine();
    }
}

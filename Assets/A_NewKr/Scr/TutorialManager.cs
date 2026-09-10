using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform villager;

    [Header("UI")]
    [SerializeField] private TutorialBubble bubble;
    [SerializeField] private TutorialArrow arrow;
    [SerializeField] private QuestLog questLog;

    [Header("Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private enum Step
    {
        Walk,
        ArrowToVillager,
        TalkToVillager1,
        LearnAttack,
        TalkToVillager2,
        Done
    }

    private Step step = Step.Walk;
    private bool entered;

    private void Update()
    {
        switch (step)
        {
            case Step.Walk:
                if (Enter()) bubble.Show("WASD — move/ SPACE — jump / E — interact / LMB — attack ");
                if (IsMoving()) { bubble.Hide(); GoTo(Step.ArrowToVillager); }
                break;

            case Step.ArrowToVillager:
                if (Enter()) arrow.PointAt(villager);
                if (NearTo(villager, interactRange)) { arrow.Hide(); GoTo(Step.TalkToVillager1); }
                break;

            case Step.TalkToVillager1:
                if (Enter()) bubble.Show("E — speak");
                if (NearTo(villager, interactRange) && Input.GetKeyDown(interactKey))
                    GoTo(Step.LearnAttack);
                break;

            case Step.LearnAttack:
                if (Enter()) bubble.Show("Villager: Press LMB to attack!");
                if (Input.GetMouseButtonDown(0)) { bubble.Hide(); GoTo(Step.TalkToVillager2); }
                break;

            case Step.TalkToVillager2:
                if (Enter()) bubble.Show("E — Speak");
                if (NearTo(villager, interactRange) && Input.GetKeyDown(interactKey))
                    GoTo(Step.Done);
                break;

            case Step.Done:
                if (Enter())
                {
                    bubble.Show("Villager: Fix the shrine!!");
                    questLog.AddQuest("Fix the shrine!");
                }
                break;
        }
    }

    private bool Enter() { if (entered) return false; entered = true; return true; }
    private void GoTo(Step next) { step = next; entered = false; }

    private bool IsMoving() =>
        Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f ||
        Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f;

    private bool NearTo(Transform target, float range)
    {
        if (target == null || player == null) return false;
        return Vector3.Distance(player.position, target.position) <= range;
    }
}
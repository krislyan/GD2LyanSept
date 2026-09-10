using UnityEngine;
using TMPro;

public class QuestLog : MonoBehaviour
{
    [SerializeField] private GameObject panel;      // панель со списком квестов
    [SerializeField] private TMP_Text listText;     // текст списка

    private void Start()
    {
        listText.text = "";
        panel.SetActive(false);   // меню старует закрытым
    }

    // повесим это на кнопку
    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void AddQuest(string title)
    {
        listText.text += "• " + title + "\n";
        panel.SetActive(true);   // при выдаче квеста меню всплывает само
    }
}
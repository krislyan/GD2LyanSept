using UnityEngine;
using TMPro;

public class TutorialBubble : MonoBehaviour
{
    [SerializeField] private GameObject root;   // сама панель пузыря
    [SerializeField] private TMP_Text label;    // текст внутри

    private void Awake() => Hide();

    public void Show(string message)
    {
        root.SetActive(true);
        label.text = message;
    }

    public void Hide() => root.SetActive(false);
}

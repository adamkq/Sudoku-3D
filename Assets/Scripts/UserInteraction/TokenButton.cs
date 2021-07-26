using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TokenButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TokenMenuManager tokenMenuManager;

    private string buttonText;

    void Awake()
    {
        button.onClick.AddListener(TaskOnClick);
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>().text;
    }

    void TaskOnClick()
    {
        tokenMenuManager.MakeSelection(char.Parse(buttonText));
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TokenButton : MonoBehaviour
{
    [SerializeField] private Button button;
    public TokenMenuManager tokenMenuManager { get; set; }
    public string ButtonText { get; set; }

    void Awake()
    {
        button.onClick.AddListener(TaskOnClick);
        ButtonText = button.GetComponentInChildren<TextMeshProUGUI>().text;
    }

    void TaskOnClick()
    {
        char chr;
        bool result = char.TryParse(ButtonText, out chr);

        if (result)
        {
            tokenMenuManager.MakeSelection(chr);
            return;
        }
        tokenMenuManager.MakeSelection(' ');
    }

    public string GetTokenValue()
    {
        return gameObject.GetComponentInChildren<TextMeshProUGUI>().text;
    }

    public void SetBackgroundColor(Color color)
    {
        ColorBlock colorBlock = button.colors;

        colorBlock.normalColor = color;

        button.colors = colorBlock;
    }
}

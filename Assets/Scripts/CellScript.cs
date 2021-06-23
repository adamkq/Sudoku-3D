using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellScript : MonoBehaviour
{
    // Script will do multiple things:
    // 1. Allow other scripts to set component properties
    // 2. Show a popup menu when the cell is tapped
    // 3. Update the character in response to popup menu selection

    [SerializeField] private Button button;
    
    private RectTransform rt;
    private TextMeshProUGUI buttonText;

    public char CellChar { get; set; }

    void Awake()
    {
        rt = gameObject.GetComponent<RectTransform>();
    }

    private void Start()
    {
        button.onClick.AddListener(TaskOnClick);
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        CellChar = '0';
    }

    private void TaskOnClick()
    {
        int currentValue;
        bool check = int.TryParse(CellChar.ToString(), out currentValue);

        if (check)
        {
            currentValue = (currentValue + 1) % 9;
            CellChar = (char)(currentValue + 48); // ASCII offset
        }

        SetButtonCharacterText(CellChar);
    }

    public void SetRectTransformSize(int width, int height)
    {
        rt.sizeDelta = new Vector2(width, height);
    }

    public void SetButtonCharacterText(char chr)
    {
        buttonText.text = chr.ToString();
    }
}

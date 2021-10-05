using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellController : MonoBehaviour
{
    // Script will do multiple things:
    // 1. Allow other scripts to set component properties
    // 2. Show a popup menu when the cell is tapped
    // 3. Update the character in response to popup menu selection

    [SerializeField] private Button button;
    [SerializeField] private Image image;
    
    private RectTransform rt;
    private TextMeshProUGUI buttonText;

    public MasterController mc { get; set; }
    public BoardManager _bm { get; set; }
    public int[] CellIndex = new int[3];
    public char CellChar { get; set; }

    void Awake()
    {
        rt = gameObject.GetComponent<RectTransform>();
        button.onClick.AddListener(TaskOnClick);
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
    }

    void TaskOnClick()
    {
        if (mc.stateManager.IsGiven(CellIndex)) return;
        GetComponent<TokenMenuManager>().OpenMenuAtCell(gameObject);
    }

    public void SetRectTransformSize(int width, int height)
    {
        rt.sizeDelta = new Vector2(width, height);
    }

    // this is called by BoardManager whenever the slice is updated
    public void SetCellIndex(int[] cellIndex)
    {
        if (cellIndex.Length != 3)
        {
            Debug.LogError("Invalid dimensions in cell index");
            return;
        }

        foreach(int index in cellIndex)
        {
            if (index < 0 || index > 7)
            {
                Debug.LogError("Invalid dimensions in cell index");
                return;
            }
        }

        this.CellIndex = cellIndex;
    }

    public void UpdateCellValue()
    {
        UpdateCellValue(CellIndex);
    }

    public void UpdateCellValue(int[] cellIndex)
    {
        CellChar = mc.stateManager.GetCellValue(cellIndex);
        buttonText.text = CellChar.ToString();
    }

    public void SetBackgroundColor(Color color)
    {
        image.color = color;
    }
}

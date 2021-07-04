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
    
    private RectTransform rt;
    private TextMeshProUGUI buttonText;
    private int[] cellIndex = new int[3];

    public MasterController mc { get; set; }
    public char CellChar { get; set; }

    void Awake()
    {
        rt = gameObject.GetComponent<RectTransform>();
    }

    private void Start()
    {
        button.onClick.AddListener(TaskOnClick);
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void TaskOnClick()
    {
        Debug.Log("Click:" + CellChar);

        UpdateCellValue(cellIndex);
        SetButtonCharacterText(CellChar);
    }

    public void SetRectTransformSize(int width, int height)
    {
        rt.sizeDelta = new Vector2(width, height);
    }

    public void SetButtonCharacterText(char chr)
    {
        Debug.LogFormat("Cell at {0}: {1}", string.Join(", ", cellIndex), CellChar);
        buttonText.text = CellChar.ToString();
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

        this.cellIndex = cellIndex;
    }

    public void UpdateCellValue(int[] cellIndex)
    {
        this.CellChar = mc.GetCellValue(cellIndex);
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CellController : MonoBehaviour
{
    // Script will do multiple things:
    // 1. Allow other scripts to set component properties
    // 2. Show a popup menu when the cell is tapped
    // 3. Update the character in response to popup menu selection

    public MasterController MasterController { get; set; }
    public BoardManager BoardManager;
    public int[] CellIndex = new int[3];
    public char CellChar { get; set; }

    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [Tooltip("Used by OnValidate to set the cell as given/not given")]
    [SerializeField] private bool cellIsGiven;

    private TextMeshProUGUI buttonText;

    public void TaskOnClick()
    {
        // shift-click to toggle givens
        if (Input.GetKey(KeyCode.LeftShift))
        {
            ToggleCellIsGiven();
        }
        else
        {
            // givens can't be changed, so no need to open a menu
            if (MasterController.stateManager.IsGiven(CellIndex)) return;
            BoardManager.OpenTokenMenuAtCell(gameObject);
        }
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

        CellIndex = cellIndex;
    }

    public void UpdateCellValue()
    {
        UpdateCellValue(CellIndex);
    }

    public void UpdateCellValue(int[] cellIndex)
    {
        CellChar = MasterController.stateManager.GetCellValue(cellIndex);
        buttonText.text = CellChar.ToString();
    }

    public void SetBackgroundColor(Color color)
    {
        image.color = color;
    }

    public void SetTint(Color color)
    {
        ColorBlock colorBlock = button.colors;

        colorBlock.normalColor = color;
        colorBlock.highlightedColor = color;
        colorBlock.pressedColor = color;
        colorBlock.selectedColor = color;

        button.colors = colorBlock;
    }

    void Awake()
    {
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
    }

    void ToggleCellIsGiven()
    {
        MasterController.stateManager.BoardGivens[CellIndex[0], CellIndex[1], CellIndex[2]] = MasterController.stateManager.IsGiven(CellIndex) ? 0 : 1;

        SetBackgroundColor(BoardManager.GetCellColor(new HashSet<char>() { 'X' }, 'X', MasterController.stateManager.IsGiven(CellIndex)));
    }
}
